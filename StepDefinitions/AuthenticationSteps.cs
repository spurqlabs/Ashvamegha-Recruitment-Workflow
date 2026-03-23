using AutomationFramework.Pages;
using AutomationFramework.Utilities;
using FluentAssertions;
using Microsoft.Playwright;
using Reqnroll;

namespace AutomationFramework.StepDefinitions;

[Binding]
public sealed class AuthenticationSteps : StepDefinitionBase
{
    private readonly LoginPage _loginPage;
    private readonly IPage _page;
    private readonly LocatorReader _locatorReader;

    public AuthenticationSteps(ScenarioContext scenarioContext) : base(scenarioContext)
    {
        _page = scenarioContext.Get<IPage>();
        _locatorReader = scenarioContext.Get<LocatorReader>();
        _loginPage = new LoginPage(_page, _locatorReader);
    }

    [Given("the user launches the application")]
    public async Task GivenTheUserLaunchesTheApplication()
    {
        await Workflow.LaunchApplicationAsync();
    }

    [Given("the user logs in with valid credentials")]
    [When("the user logs in with valid credentials")]
    public async Task WhenTheUserLogsInWithValidCredentials()
    {
        await Workflow.LoginAsync();
    }

    [Then("the dashboard should be displayed")]
    [Then("the dashboard is displayed")]
    public async Task ThenTheDashboardShouldBeDisplayed()
    {
        (await _loginPage.IsDashboardVisibleAsync())
            .Should()
            .BeTrue("the user should land on an authenticated page after a successful login");
    }

    [When("the user logs out of the application")]
    public async Task WhenTheUserLogsOutOfTheApplication()
    {
        FrameworkLogger.Info("Logging out of the application.");
        await _page.Locator(_locatorReader.Get("Common", "userDropdown")).ClickAsync();
        await _page.Locator(_locatorReader.Get("Common", "logoutLink")).ClickAsync();
    }

    [Then("the login page should be displayed")]
    public async Task ThenTheLoginPageShouldBeDisplayed()
    {
        (await _loginPage.IsLoginVisibleAsync())
            .Should()
            .BeTrue("the login page header should be visible after logout");
    }
}
