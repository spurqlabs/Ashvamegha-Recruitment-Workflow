using AutomationFramework.Utilities;
using Microsoft.Playwright;

namespace AutomationFramework.Pages;

public class LoginPage : BasePage
{
    public LoginPage(IPage page, LocatorReader locatorReader) : base(page, locatorReader)
    {
    }

    public async Task NavigateAsync(string url)
    {
        try
        {
            await Page.GotoAsync(url, new PageGotoOptions
            {
                WaitUntil = WaitUntilState.Commit,
                Timeout = 60000
            });
        }
        catch (TimeoutException)
        {
            FrameworkLogger.Warn($"Navigation timed out for '{url}'. Retrying once with a lighter wait strategy.");
            await Page.GotoAsync(url, new PageGotoOptions
            {
                WaitUntil = WaitUntilState.Commit,
                Timeout = 30000
            });
        }

        await Locator("LoginPage", "usernameInput").WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 30000
        });
    }

    public async Task LoginAsync(string username, string password)
    {
        await FillAsync("LoginPage", "usernameInput", username);
        await FillAsync("LoginPage", "passwordInput", password);
        await ClickAsync("LoginPage", "loginButton");
    }

    public async Task<bool> IsDashboardVisibleAsync()
    {
        var dashboardVisible = await Locator("LoginPage", "dashboardHeader").IsVisibleAsync();
        if (dashboardVisible)
        {
            return true;
        }

        var userDropdownVisible = await Locator("Common", "userDropdown").IsVisibleAsync();
        if (userDropdownVisible)
        {
            return true;
        }

        return !Page.Url.Contains("/auth/login", StringComparison.OrdinalIgnoreCase);
    }

    public async Task<bool> IsLoginVisibleAsync()
    {
        return await Locator("Common", "loginHeader").IsVisibleAsync();
    }
}
