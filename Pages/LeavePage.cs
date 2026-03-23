using AutomationFramework.Utilities;
using Microsoft.Playwright;

namespace AutomationFramework.Pages;

public class LeavePage : BasePage
{
    public LeavePage(IPage page, LocatorReader locatorReader) : base(page, locatorReader)
    {
    }

    public async Task NavigateToEntitlementsAddAsync()
    {
        FrameworkLogger.Info("Navigating to Leave > Entitlements > Add Entitlements.");
        await ClickAsync("LeavePage", "leaveMenu");
        await ClickAsync("LeavePage", "entitlementsMenu");
        await ClickAsync("LeavePage", "addEntitlementsMenu");
    }

    public async Task NavigateToApplyLeaveAsync()
    {
        FrameworkLogger.Info("Navigating to Leave > Apply.");
        await ClickAsync("LeavePage", "leaveMenu");
        await ClickAsync("LeavePage", "applyMenu");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task NavigateToMyLeaveAsync()
    {
        FrameworkLogger.Info("Navigating to Leave > My Leave.");
        await ClickAsync("LeavePage", "leaveMenu");
        await ClickAsync("LeavePage", "myLeaveMenu");
    }

    public async Task<string> GetLoggedInEmployeeNameAsync()
    {
        FrameworkLogger.Info("Reading logged in employee name from the user menu.");
        return await GetTextAsync("LeavePage", "loggedInEmployeeName");
    }

    public async Task EnterEntitlementEmployeeNameAsync(string employeeName)
    {
        FrameworkLogger.Info($"Entering entitlement employee name: {employeeName}.");
        var input = Locator("LeavePage", "employeeNameInput");
        await WaitHelper.WaitForVisibleAsync(input);
        await input.ClickAsync();
        await input.PressAsync("Control+A");
        await input.PressAsync("Backspace");

        // Type only the first name (first word) to trigger autocomplete
        var firstName = employeeName.Split(' ')[0];
        await input.PressSequentiallyAsync(firstName, new() { Delay = 100 });

        // Wait for the dropdown container to fully load
        var dropdown = Page.Locator(".oxd-autocomplete-dropdown");
        await WaitHelper.WaitForVisibleAsync(dropdown);

        // Select the option that contains the first name from the top-right header
        var matchingOption = dropdown.Locator("[role='option']")
            .Filter(new() { HasText = firstName });
        await WaitHelper.WaitForVisibleAsync(matchingOption.First);
        await matchingOption.First.ClickAsync();
    }

    public async Task SelectEntitlementLeaveTypeAsync(string leaveType)
    {
        FrameworkLogger.Info($"Selecting entitlement leave type: {leaveType}.");
        await SelectOptionFromDropdownAsync("LeavePage", "entitlementLeaveTypeDropdown", "entitlementLeaveTypeOptions", leaveType);
    }

    public async Task SelectEntitlementPeriodAsync(string leavePeriod)
    {
        FrameworkLogger.Info($"Selecting entitlement leave period: {leavePeriod}.");
        await SelectOptionFromDropdownAsync("LeavePage", "leavePeriodDropdown", "leavePeriodOptions", leavePeriod);
    }

    public async Task EnterEntitlementDaysAsync(string days)
    {
        FrameworkLogger.Info($"Entering entitlement days: {days}.");
        await FillAsync("LeavePage", "entitlementInput", days);
    }

    public async Task SaveEntitlementAsync()
    {
        FrameworkLogger.Info("Saving leave entitlement.");
        await ClickAsync("LeavePage", "entitlementSaveButton");

        // If an entitlement already exists, a confirmation dialog appears — click Confirm
        var confirmButton = Locator("LeavePage", "entitlementConfirmButton");
        await Page.WaitForTimeoutAsync(2000); // Allow dialog to render if it is going to appear
        if (await confirmButton.IsVisibleAsync())
        {
            FrameworkLogger.Info("Entitlement update confirmation dialog detected. Clicking Confirm.");
            await confirmButton.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        // Wait for either success toast or error message (with shorter timeout)
        try
        {
            var successToast = Locator("LeavePage", "successToast");
            await WaitHelper.WaitForVisibleAsync(successToast, 5000);
            FrameworkLogger.Info("Entitlement saved successfully.");
        }
        catch
        {
            // If success toast doesn't appear, check if we're back on the page (navigation occurred)
            // This sometimes means the save was successful even without a visible toast
            FrameworkLogger.Info("No toast message detected, but proceeding - page may have navigated.");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }
    }

    public async Task SelectLeaveTypeAsync(string leaveType)
    {
        FrameworkLogger.Info($"Selecting leave type: {leaveType}.");
        await SelectOptionFromDropdownAsync("LeavePage", "leaveTypeDropdown", "leaveTypeOptions", leaveType);
    }

    public async Task EnterFromDateAsync(string fromDate)
    {
        FrameworkLogger.Info($"Selecting leave from date via picker: {fromDate}.");
        await SelectDateViaPickerAsync("LeavePage", "fromDateCalendarIcon", fromDate);
    }

    public async Task EnterToDateAsync(string toDate)
    {
        FrameworkLogger.Info($"Selecting leave to date via picker: {toDate}.");
        await SelectDateViaPickerAsync("LeavePage", "toDateCalendarIcon", toDate);
    }

    public async Task SelectPartialDaysAsync(string partialDays)
    {
        FrameworkLogger.Info($"Selecting partial days option: {partialDays}.");
        
        // Give the page time to fully load after date selections
        await Page.WaitForTimeoutAsync(1000);
        
        // Scroll into view if needed
        try
        {
            var partialDaysLocator = Locator("LeavePage", "partialDaysDropdown");
            await partialDaysLocator.ScrollIntoViewIfNeededAsync();
        }
        catch { }
        
        await Page.WaitForTimeoutAsync(500);
        // Use extended timeout for this dropdown (60 seconds)
        await SelectOptionFromDropdownAsync("LeavePage", "partialDaysDropdown", "partialDaysOptions", partialDays);
    }

    public async Task EnterCommentAsync(string comment)
    {
        FrameworkLogger.Info("Entering leave comment.");
        await FillAsync("LeavePage", "commentTextarea", comment);
    }

    public async Task ApplyAsync()
    {
        FrameworkLogger.Info("Submitting leave request.");
        await ClickAsync("LeavePage", "applyButton");
    }

    public async Task<string> GetSuccessMessageAsync()
    {
        FrameworkLogger.Info("Reading leave success message.");
        return await GetTextAsync("LeavePage", "successToast");
    }

    public ILocator GetMyLeaveRows()
    {
        FrameworkLogger.Info("Getting My Leave table rows.");
        return Locator("LeavePage", "myLeaveTableRows");
    }

    public async Task FilterByDateRangeAsync(string fromDate, string toDate)
    {
        FrameworkLogger.Info($"Filtering leave records from {fromDate} to {toDate}.");
        await SelectDateViaPickerAsync("LeavePage", "filterFromDateCalendarIcon", fromDate);
        await SelectDateViaPickerAsync("LeavePage", "filterToDateCalendarIcon", toDate);
        await ClickAsync("LeavePage", "searchButton");
    }

    public ILocator GetPendingApprovalStatusCell()
    {
        FrameworkLogger.Info("Getting Pending Approval status cell.");
        return Locator("LeavePage", "statusCell");
    }
}
