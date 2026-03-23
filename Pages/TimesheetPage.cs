using AutomationFramework.Utilities;
using Microsoft.Playwright;

namespace AutomationFramework.Pages;

public class TimesheetPage : BasePage
{
    public TimesheetPage(IPage page, LocatorReader locatorReader) : base(page, locatorReader)
    {
    }

    public async Task NavigateToMyTimesheetsAsync()
    {
        FrameworkLogger.Info("Navigating to Time > Timesheets > My Timesheets.");
        await ClickAsync("TimesheetPage", "timeMenu");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await ClickAsync("TimesheetPage", "timesheetsTopMenu");
        await ClickAsync("TimesheetPage", "myTimesheetsMenu");
    }

    public async Task StartAddingEntryAsync()
    {
        FrameworkLogger.Info("Opening add timesheet entry form.");
        await ClickAsync("TimesheetPage", "addRowButton");
    }

    public async Task SelectProjectAsync(string project)
    {
        FrameworkLogger.Info($"Selecting timesheet project: {project}.");
        await SelectOptionFromDropdownAsync("TimesheetPage", "projectDropdown", "projectOptions", project);
    }

    public async Task SelectActivityAsync(string activity)
    {
        FrameworkLogger.Info($"Selecting timesheet activity: {activity}.");
        await SelectOptionFromDropdownAsync("TimesheetPage", "activityDropdown", "activityOptions", activity);
    }

    public async Task EnterHoursAsync(string hours)
    {
        FrameworkLogger.Info($"Entering timesheet hours: {hours}.");
        await FillAsync("TimesheetPage", "hoursInput", hours);
    }

    public async Task SaveAsync()
    {
        FrameworkLogger.Info("Saving timesheet entry.");
        await ClickAsync("TimesheetPage", "saveButton");
    }

    public async Task<string> GetSuccessMessageAsync()
    {
        FrameworkLogger.Info("Reading timesheet success message.");
        return await GetTextAsync("TimesheetPage", "successToast");
    }

    public ILocator GetTimesheetRows()
    {
        FrameworkLogger.Info("Getting timesheet table rows.");
        return Locator("TimesheetPage", "timesheetTableRows");
    }

    public async Task<string> GetTotalHoursTextAsync()
    {
        FrameworkLogger.Info("Reading timesheet total hours text.");
        return await GetTextAsync("TimesheetPage", "totalHoursValue");
    }
}
