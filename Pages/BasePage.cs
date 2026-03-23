using AutomationFramework.Utilities;
using Microsoft.Playwright;
using System.Text.RegularExpressions;

namespace AutomationFramework.Pages;

public abstract class BasePage
{
    protected readonly IPage Page;
    protected readonly LocatorReader LocatorReader;

    protected BasePage(IPage page, LocatorReader locatorReader)
    {
        Page = page;
        LocatorReader = locatorReader;
    }

    protected ILocator Locator(string section, string key) => Page.Locator(LocatorReader.Get(section, key));

    protected async Task ClickAsync(string section, string key)
    {
        var locator = Locator(section, key);
        await WaitHelper.WaitForVisibleAsync(locator);
        await locator.ClickAsync();
    }

    protected async Task FillAsync(string section, string key, string value)
    {
        var locator = Locator(section, key);
        await WaitHelper.WaitForVisibleAsync(locator);
        await locator.FillAsync(value);
    }

    protected async Task<string> GetTextAsync(string section, string key)
    {
        var locator = Locator(section, key);
        await WaitHelper.WaitForVisibleAsync(locator);
        return (await locator.InnerTextAsync()).Trim();
    }

    protected async Task UploadFileAsync(string section, string key, string relativePath)
    {
        var fullPath = PathHelper.ResolveFromProject(relativePath);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"Upload file not found at path: {fullPath}");
        }

        await Locator(section, key).SetInputFilesAsync(fullPath);
    }

    protected async Task SelectOptionFromDropdownAsync(string section, string dropdownKey, string optionsKey, string value)
    {
        await ClickAsync(section, dropdownKey);

        var options = Page.Locator(LocatorReader.Get(section, optionsKey));
        await WaitHelper.WaitForVisibleAsync(options.First);

        var expectedValue = NormalizeText(value);
        var optionCount = await options.CountAsync();

        for (var i = 0; i < optionCount; i++)
        {
            var option = options.Nth(i);
            var optionText = NormalizeText(await option.InnerTextAsync());

            if (optionText.Equals(expectedValue, StringComparison.OrdinalIgnoreCase))
            {
                await option.ClickAsync();
                return;
            }
        }

        var availableOptions = new List<string>();
        for (var i = 0; i < optionCount; i++)
        {
            var optionText = NormalizeText(await options.Nth(i).InnerTextAsync());
            if (!string.IsNullOrWhiteSpace(optionText))
            {
                availableOptions.Add(optionText);
            }
        }

        throw new InvalidOperationException(
            $"Dropdown option '{value}' was not found in section '{section}' for key '{dropdownKey}'. " +
            $"Available options: {string.Join(", ", availableOptions)}");
    }

    private static string NormalizeText(string value) =>
        Regex.Replace(value ?? string.Empty, @"\s+", " ").Trim();

    /// <summary>
    /// Selects a date from the OrangeHRM date picker. First tries to fill the input directly,
    /// <summary>
    /// Selects a date by typing it into the date input field.
    /// </summary>
    protected async Task SelectDateViaPickerAsync(string section, string calendarIconKey, string isoDate)
    {
        var date = DateTime.Parse(isoDate);
        // Format: dd-MM-yyyy (common for OrangeHRM)
        string formattedDate = date.ToString("dd-MM-yyyy");
        
        try
        {
            // Close any open pop-ups
            await Page.Keyboard.PressAsync("Escape");
            await Page.WaitForTimeoutAsync(100);
        }
        catch { }

        // Find the date input field near the calendar icon
        var iconLocator = Locator(section, calendarIconKey);
        
        // Try to find the input element in the same container
        var container = Page.Locator(LocatorReader.Get(section, calendarIconKey)).Locator("..");
        var dateInputs = container.Locator("input");
        
        var count = await dateInputs.CountAsync();
        if (count > 0)
        {
            var dateInput = dateInputs.First;
            
            // Clear and type the date
            await dateInput.FillAsync("");
            await Page.WaitForTimeoutAsync(100);
            await dateInput.TypeAsync(formattedDate, new() { Delay = 50 });
            await Page.WaitForTimeoutAsync(200);
            
            // Trigger change event
            await dateInput.PressAsync("Tab");
            await Page.WaitForTimeoutAsync(300);
        }
        
        // Close any open calendar
        try
        {
            await Page.Keyboard.PressAsync("Escape");
            await Page.WaitForTimeoutAsync(100);
        }
        catch { }
    }
}
