 using Reqnroll;

namespace AutomationFramework.StepDefinitions;

[Binding]

public sealed class CandidateCreationSteps : StepDefinitionBase
{
    public CandidateCreationSteps(ScenarioContext scenarioContext) : base(scenarioContext)
    {
    }

    [Given("the user is logged in and navigated to Recruitment candidates")]
    public async Task GivenTheUserIsLoggedInAndNavigatedToRecruitmentCandidates()
    {
        await Workflow.LaunchApplicationAsync();
        await Workflow.LoginAsync();
        await Workflow.NavigateToCandidatesAsync();
    }

    [When("the user navigates to Recruitment candidates")]
    public async Task WhenTheUserNavigatesToRecruitmentCandidates()
    {
        await Workflow.NavigateToCandidatesAsync();
    }

    [Given("an existing candidate record is available for the test")]
    public async Task GivenAnExistingCandidateRecordIsAvailableForTheTest()
    {
        await Workflow.CreateCandidateAsync();
        await Workflow.NavigateToCandidatesAsync();
    }

    [When("the user starts adding a new candidate")]
    public async Task WhenTheUserStartsAddingANewCandidate()
    {
        await Workflow.StartCandidateCreationAsync();
    }

    [When("the user enters all required candidate details")]
    public async Task WhenTheUserEntersAllRequiredCandidateDetails()
    {
        await Workflow.EnterCandidateDetailsAsync();
    }

    [When("the user saves the candidate")]
    public async Task WhenTheUserSavesTheCandidate()
    {
        await Workflow.SaveCandidateAsync();
    }

    [Then("candidate creation should be successful")]
    [Then("the candidate creation success message is displayed")]
    public async Task ThenCandidateCreationIsSuccessful()
    {
        await Workflow.ValidateCandidateCreationSuccessAsync();
    }

    [Then("the candidate profile should be available")]
    [Then("the candidate record details are displayed")]
    public Task ThenTheCandidateProfileShouldBeAvailable()
    {
        ValidateCandidateProfile();
        return Task.CompletedTask;
    }
}
