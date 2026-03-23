using AutomationFramework.Pages;
using AutomationFramework.Utilities;
using FluentAssertions;
using Microsoft.Playwright;
using Newtonsoft.Json.Linq;
using Reqnroll;

namespace AutomationFramework.StepDefinitions;

[Binding]
public sealed class LeaveSteps : StepDefinitionBase
{
    private readonly LeavePage _leavePage;
    private readonly JObject _scenarioData;
    private readonly IPage _page;

    public LeaveSteps(ScenarioContext scenarioContext) : base(scenarioContext)
    {
        var page = scenarioContext.Get<IPage>();
        var locatorReader = scenarioContext.Get<LocatorReader>();

        _leavePage = new LeavePage(page, locatorReader);
        _scenarioData = scenarioContext.Get<JObject>();
        _page = page;
    }

    [Given("the user is logged in")]
    public async Task GivenTheUserIsLoggedIn()
    {
        FrameworkLogger.Info("Launching application and logging in.");
        await Workflow.LaunchApplicationAsync();
        await Workflow.LoginAsync();

        Config.ApplicationUrl
            .Should()
            .NotBeNullOrWhiteSpace("application url should be configured before leave workflow starts");
    }

    [When("the user adds leave entitlement using the logged in employee name")]
    public async Task WhenTheUserAddsLeaveEntitlementUsingTheLoggedInEmployeeName()
    {
        var entitlement = _scenarioData["entitlement"]
            ?? throw new KeyNotFoundException("Scenario data is missing the 'entitlement' section.");

        await _leavePage.NavigateToEntitlementsAddAsync();

        var employeeName = await _leavePage.GetLoggedInEmployeeNameAsync();
        employeeName.Should().NotBeNullOrWhiteSpace("logged in employee name should be visible in the top right user menu");

        await _leavePage.EnterEntitlementEmployeeNameAsync(employeeName);
        await _leavePage.SelectEntitlementLeaveTypeAsync(entitlement.Value<string>("leaveType")!);

        var entitlementPeriod = entitlement.Value<string>("period");
        if (!string.IsNullOrWhiteSpace(entitlementPeriod))
        {
            await _leavePage.SelectEntitlementPeriodAsync(entitlementPeriod);
        }

        await _leavePage.EnterEntitlementDaysAsync(entitlement.Value<string>("days")!);
        await _leavePage.SaveEntitlementAsync();
    }

    [When("the user navigates to Leave Apply")]
    public async Task WhenTheUserNavigatesToLeaveApply()
    {
        await _leavePage.NavigateToApplyLeaveAsync();
    }

    [When("the user fills the leave application form with data from JSON")]
    public async Task WhenTheUserFillsTheLeaveApplicationFormWithDataFromJson()
    {
        var leaveDataPath = PathHelper.ResolveFromProject("TestData/leaveData.json");

        FrameworkLogger.Info($"Reading leave test data from {leaveDataPath}.");
        File.Exists(leaveDataPath)
            .Should()
            .BeTrue("leave test data file should exist before filling the leave application form");

        var leave = _scenarioData["leave"] ?? throw new KeyNotFoundException("Scenario data is missing the 'leave' section.");

        await _leavePage.SelectLeaveTypeAsync(leave.Value<string>("type")!);
        await _leavePage.EnterFromDateAsync(leave.Value<string>("fromDate")!);
        await _leavePage.EnterToDateAsync(leave.Value<string>("toDate")!);
        
        // Wait for page to fully settle after date selections
        await _page.WaitForTimeoutAsync(1500);
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var partialDays = leave.Value<string>("partialDays");
        if (!string.IsNullOrWhiteSpace(partialDays))
        {
            await _leavePage.SelectPartialDaysAsync(partialDays);
        }

        await _leavePage.EnterCommentAsync(leave.Value<string>("comment")!);
    }

    [When("the user submits the leave request")]
    public async Task WhenTheUserSubmitsTheLeaveRequest()
    {
        await _leavePage.ApplyAsync();
    }

    [Then("the leave success message is displayed")]
    public async Task ThenTheLeaveSuccessMessageIsDisplayed()
    {
        var successMessage = await _leavePage.GetSuccessMessageAsync();

        successMessage
            .Should()
            .Contain("Success", "a success toast should be shown after submitting leave request");
    }

    [When("the user navigates to My Leave")]
    public async Task WhenTheUserNavigatesToMyLeave()
    {
        await _leavePage.NavigateToMyLeaveAsync();
    }

    [Then("the applied leave should appear in My Leave")]
    public async Task ThenTheAppliedLeaveShouldAppearInMyLeave()
    {
        var rows = _leavePage.GetMyLeaveRows();
        await WaitHelper.WaitForVisibleAsync(rows.First);
        (await rows.CountAsync())
            .Should()
            .BeGreaterThan(0, "at least one leave record should be visible in My Leave");
    }

    [When("the user filters leave by the configured date range")]
    public async Task WhenTheUserFiltersLeaveByTheConfiguredDateRange()
    {
        var filter = _scenarioData["filter"] ?? throw new KeyNotFoundException("Scenario data is missing the 'filter' section.");

        await _leavePage.FilterByDateRangeAsync(
            filter.Value<string>("fromDate")!,
            filter.Value<string>("toDate")!);
    }

    [Then("the leave status should be Pending Approval")]
    public async Task ThenTheLeaveStatusShouldBePendingApproval()
    {
        var expectedStatus = _scenarioData["leave"]?.Value<string>("status") ?? "Pending Approval";
        var statusCell = _leavePage.GetPendingApprovalStatusCell();

        (await statusCell.First.IsVisibleAsync())
            .Should()
            .BeTrue($"leave status should be '{expectedStatus}' after filtering");
    }
}
