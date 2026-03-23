using AutomationFramework.Models;
using AutomationFramework.Utilities;
using FluentAssertions;
using Reqnroll;

namespace AutomationFramework.StepDefinitions;

public abstract class StepDefinitionBase
{
    protected readonly ScenarioContext ScenarioContext;
    protected readonly ConfigModel Config;
    protected readonly CandidateDataModel TestData;
    protected readonly RuntimeCandidateContext RuntimeCandidateContext;
    protected readonly RecruitmentWorkflow Workflow;

    protected StepDefinitionBase(ScenarioContext scenarioContext)
    {
        ScenarioContext = scenarioContext;
        Config = scenarioContext.Get<ConfigModel>();
        TestData = scenarioContext.Get<CandidateDataModel>();
        RuntimeCandidateContext = scenarioContext.Get<RuntimeCandidateContext>();

        var page = scenarioContext.Get<Microsoft.Playwright.IPage>();
        var locatorReader = scenarioContext.Get<LocatorReader>();

        Workflow = new RecruitmentWorkflow(page, locatorReader, Config, TestData, RuntimeCandidateContext);
    }

    protected void ValidateCandidateProfile()
    {
        Workflow.ValidateCandidateProfileLoadedAsync().GetAwaiter().GetResult();

        RuntimeCandidateContext.FullName
            .Should()
            .NotBeNullOrWhiteSpace("candidate full name should be available after opening the candidate profile");

        RuntimeCandidateContext.Email
            .Should()
            .NotBeNullOrWhiteSpace("candidate email should be available after opening the candidate profile");
    }
}
