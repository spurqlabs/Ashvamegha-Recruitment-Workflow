using AutomationFramework.Pages;
using AutomationFramework.Utilities;
using FluentAssertions;
using Microsoft.Playwright;
using Newtonsoft.Json.Linq;
using Reqnroll;

namespace AutomationFramework.StepDefinitions;

[Binding]
public sealed class TimesheetSteps : StepDefinitionBase
{
    private readonly TimesheetPage _timesheetPage;
    private readonly JObject _scenarioData;

    public TimesheetSteps(ScenarioContext scenarioContext) : base(scenarioContext)
    {
        var page = scenarioContext.Get<IPage>();
        var locatorReader = scenarioContext.Get<LocatorReader>();

        _timesheetPage = new TimesheetPage(page, locatorReader);
        _scenarioData = scenarioContext.Get<JObject>();
    }

    [Given("the user is logged in and navigated to My Timesheets")]
    public async Task GivenTheUserIsLoggedInAndNavigatedToMyTimesheets()
    {
        FrameworkLogger.Info("Launching application and logging in before navigating to My Timesheets.");
        await Workflow.LaunchApplicationAsync();
        await Workflow.LoginAsync();
        await _timesheetPage.NavigateToMyTimesheetsAsync();

        Config.ApplicationUrl
            .Should()
            .NotBeNullOrWhiteSpace("application url should be configured before navigation starts");
    }

    [When("the user starts adding a new timesheet entry")]
    public async Task WhenTheUserStartsAddingANewTimesheetEntry()
    {
        await _timesheetPage.StartAddingEntryAsync();
    }

    [When("the user enters the project, activity, and hours for the timesheet entry")]
    public async Task WhenTheUserEntersTheProjectActivityAndHoursForTheTimesheetEntry()
    {
        var timesheetDataPath = PathHelper.ResolveFromProject("TestData/timesheetData.json");

        FrameworkLogger.Info($"Reading timesheet test data from {timesheetDataPath}.");
        File.Exists(timesheetDataPath)
            .Should()
            .BeTrue("timesheet test data file should exist before entering timesheet details");

        var timesheet = _scenarioData["timesheet"] ?? throw new KeyNotFoundException("Scenario data is missing the 'timesheet' section.");

        await _timesheetPage.SelectProjectAsync(timesheet.Value<string>("project")!);
        await _timesheetPage.SelectActivityAsync(timesheet.Value<string>("activity")!);
        await _timesheetPage.EnterHoursAsync(timesheet.Value<string>("hours")!);
    }

    [When("the user saves the timesheet")]
    public async Task WhenTheUserSavesTheTimesheet()
    {
        await _timesheetPage.SaveAsync();
    }

    [Then("the timesheet success message is displayed")]
    public async Task ThenTheTimesheetSuccessMessageIsDisplayed()
    {
        var successMessage = await _timesheetPage.GetSuccessMessageAsync();

        successMessage
            .Should()
            .Contain("Success", "a success toast should be shown after saving the timesheet");
    }

    [Then("the timesheet entry appears in the table")]
    public async Task ThenTheTimesheetEntryAppearsInTheTable()
    {
        var rows = _timesheetPage.GetTimesheetRows();
        (await rows.CountAsync())
            .Should()
            .BeGreaterThan(0, "at least one timesheet row should be visible after saving the entry");
    }

    [Then("the total hours should be calculated correctly")]
    public async Task ThenTheTotalHoursShouldBeCalculatedCorrectly()
    {
        var expectedTotal = _scenarioData["timesheet"]?.Value<string>("expectedTotalHours");
        var totalHoursText = await _timesheetPage.GetTotalHoursTextAsync();

        totalHoursText
            .Should()
            .Contain(expectedTotal, "the total hours summary should reflect the saved timesheet hours");
    }
}
