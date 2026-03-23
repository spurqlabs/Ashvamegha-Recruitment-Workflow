using AutomationFramework.Pages;
using AutomationFramework.Utilities;
using FluentAssertions;
using Microsoft.Playwright;
using Reqnroll;

namespace AutomationFramework.StepDefinitions;

[Binding]
public sealed class CandidateStatusSteps : StepDefinitionBase
{
    private readonly CandidateListPage _candidateListPage;

    public CandidateStatusSteps(ScenarioContext scenarioContext) : base(scenarioContext)
    {
        var page = scenarioContext.Get<IPage>();
        var locatorReader = scenarioContext.Get<LocatorReader>();
        _candidateListPage = new CandidateListPage(page, locatorReader);
    }

    [When("the user updates the candidate status to shortlist")]
    public async Task WhenTheUserUpdatesTheCandidateStatusToShortlist()
    {
        await _candidateListPage.UpdateStatusAsync(TestData.Candidate.StatusToUpdate, TestData.Candidate);
        RuntimeCandidateContext.CurrentStatus = TestData.Candidate.StatusToUpdate;
        FrameworkLogger.Info($"Candidate status updated to {RuntimeCandidateContext.CurrentStatus}.");
    }

    [When("the user updates the candidate status to interview scheduled")]
    public async Task WhenTheUserUpdatesTheCandidateStatusToInterviewScheduled()
    {
        await _candidateListPage.UpdateStatusAsync(TestData.Candidate.SecondaryStatusToUpdate, TestData.Candidate);
        RuntimeCandidateContext.CurrentStatus = TestData.Candidate.SecondaryStatusToUpdate;
        FrameworkLogger.Info($"Candidate status updated to {RuntimeCandidateContext.CurrentStatus}.");
    }

    [Then("the candidate status should be updated")]
    public async Task ThenTheCandidateStatusShouldBeUpdated()
    {
        var isUpdated = await _candidateListPage.IsStatusUpdatedAsync(RuntimeCandidateContext.CurrentStatus);
        isUpdated.Should().BeTrue();
    }
}
