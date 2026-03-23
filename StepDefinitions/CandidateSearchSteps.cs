using Reqnroll;

namespace AutomationFramework.StepDefinitions;

[Binding]
public sealed class CandidateSearchSteps : StepDefinitionBase
{
    public CandidateSearchSteps(ScenarioContext scenarioContext) : base(scenarioContext)
    {
    }

    [Given("the user searches for the created candidate")]
    [When("the user searches for the created candidate")]
    public async Task WhenTheUserSearchesForTheCreatedCandidate()
    {
        await Workflow.SearchCreatedCandidateAsync();
    }

    [Then("the created candidate should appear in the candidates list")]
    [Then("the created candidate is displayed in the candidates list")]
    public async Task ThenTheCreatedCandidateShouldAppearInTheCandidatesList()
    {
        await Workflow.ValidateCandidateInListAsync();
    }

    [Given("the user opens the candidate details")]
    [When("the user opens the candidate details")]
    public async Task WhenTheUserOpensTheCandidateDetails()
    {
        await Workflow.OpenCreatedCandidateAsync();
    }
}
