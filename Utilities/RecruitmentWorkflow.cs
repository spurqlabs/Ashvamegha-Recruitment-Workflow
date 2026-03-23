using AutomationFramework.Models;
using AutomationFramework.Pages;
using FluentAssertions;
using Microsoft.Playwright;

namespace AutomationFramework.Utilities;

public class RecruitmentWorkflow
{
    private readonly ConfigModel _config;
    private readonly CandidateDataModel _data;
    private readonly RuntimeCandidateContext _runtimeContext;
    private readonly LoginPage _loginPage;
    private readonly RecruitmentPage _recruitmentPage;
    private readonly AddCandidatePage _addCandidatePage;
    private readonly CandidateListPage _candidateListPage;

    private CandidateInfo? _preparedCandidate;
    private bool _candidateCreated;

    public RecruitmentWorkflow(
        IPage page,
        LocatorReader locatorReader,
        ConfigModel config,
        CandidateDataModel data,
        RuntimeCandidateContext runtimeContext)
    {
        _config = config;
        _data = data;
        _runtimeContext = runtimeContext;
        _loginPage = new LoginPage(page, locatorReader);
        _recruitmentPage = new RecruitmentPage(page, locatorReader);
        _addCandidatePage = new AddCandidatePage(page, locatorReader);
        _candidateListPage = new CandidateListPage(page, locatorReader);
    }

    public async Task LaunchApplicationAsync()
    {
        FrameworkLogger.Info("Launching application.");
        await _loginPage.NavigateAsync(_config.ApplicationUrl);
    }

    public async Task LoginAsync()
    {
        FrameworkLogger.Info("Logging in with configured credentials.");
        await _loginPage.LoginAsync(_data.Login.Username, _data.Login.Password);
        (await _loginPage.IsDashboardVisibleAsync())
            .Should()
            .BeTrue("dashboard or authenticated user menu should be visible after login");
    }

    public async Task NavigateToCandidatesAsync()
    {
        FrameworkLogger.Info("Navigating to Recruitment > Candidates.");
        await _recruitmentPage.NavigateToCandidatesAsync();
    }

    public async Task StartCandidateCreationAsync()
    {
        FrameworkLogger.Info("Opening Add Candidate form.");
        _candidateCreated = false;
        _preparedCandidate = null;
        await _addCandidatePage.ClickAddCandidateAsync();
    }

    public CandidateInfo BuildUniqueCandidate()
    {
        var baseCandidate = _data.Candidate;
        var suffix = DateTime.Now.ToString("yyyyMMddHHmmss");

        var candidate = new CandidateInfo
        {
            FirstName = baseCandidate.FirstName,
            LastName = $"{baseCandidate.LastName}{suffix[^4..]}",
            Email = baseCandidate.Email.Replace("@", $".{suffix}@"),
            Phone = baseCandidate.Phone.Length >= 10
                ? $"{baseCandidate.Phone[..6]}{suffix[^4..]}"
                : $"{baseCandidate.Phone}{suffix[^4..]}",
            Vacancy = baseCandidate.Vacancy,
            Keywords = baseCandidate.Keywords,
            Notes = $"{baseCandidate.Notes} | Run: {suffix}",
            ResumePath = baseCandidate.ResumePath,
            StatusToUpdate = baseCandidate.StatusToUpdate,
            SecondaryStatusToUpdate = baseCandidate.SecondaryStatusToUpdate
        };

        _runtimeContext.FirstName = candidate.FirstName;
        _runtimeContext.LastName = candidate.LastName;
        _runtimeContext.Email = candidate.Email;

        FrameworkLogger.Info($"Generated unique candidate data for {_runtimeContext.FullName} / {_runtimeContext.Email}.");
        return candidate;
    }

    public async Task EnterCandidateDetailsAsync()
    {
        _preparedCandidate ??= BuildUniqueCandidate();

        FrameworkLogger.Info("Entering candidate details.");
        await _addCandidatePage.AddCandidateAsync(_preparedCandidate);
    }

    public async Task SaveCandidateAsync()
    {
        FrameworkLogger.Info("Saving candidate.");
        await _addCandidatePage.SaveAsync();

        var successMessage = await _addCandidatePage.GetSuccessMessageAsync();
        successMessage
            .Should()
            .Contain("Success", "a success toast should be displayed after saving a new candidate");

        await _addCandidatePage.WaitForCandidateDetailsPageToLoadAsync();

        _candidateCreated = true;
        FrameworkLogger.Info("Candidate created successfully. Success message displayed and candidate details page loaded.");
    }

    public async Task CreateCandidateAsync()
    {
        await StartCandidateCreationAsync();
        await EnterCandidateDetailsAsync();
        await SaveCandidateAsync();
    }

    public Task ValidateCandidateCreationSuccessAsync()
    {
        _candidateCreated
            .Should()
            .BeTrue("candidate creation flow should complete only after the success message appears and the details page has loaded");
        return Task.CompletedTask;
    }

    public async Task EnsureCandidateCreatedAsync()
    {
        if (_candidateCreated && !string.IsNullOrWhiteSpace(_runtimeContext.FullName))
        {
            FrameworkLogger.Info($"Reusing existing candidate {_runtimeContext.FullName} / {_runtimeContext.Email}.");
            return;
        }

        await CreateCandidateAsync();
    }

    public async Task ValidateCandidateProfileLoadedAsync()
    {
        await _addCandidatePage.WaitForCandidateDetailsPageToLoadAsync();
        _runtimeContext.FullName
            .Should()
            .NotBeNullOrWhiteSpace("candidate full name should be stored in runtime context before profile validation");
        _runtimeContext.Email
            .Should()
            .NotBeNullOrWhiteSpace("candidate email should be stored in runtime context before profile validation");
    }

    public async Task SearchCreatedCandidateAsync()
    {
        FrameworkLogger.Info($"Searching candidate {_runtimeContext.FullName}.");
        await _candidateListPage.SearchByCandidateNameAsync(_runtimeContext.FullName);
    }

    public async Task ValidateCandidateInListAsync()
    {
        FrameworkLogger.Info($"Validating candidate {_runtimeContext.FullName} is visible in list.");
        (await _candidateListPage.IsCandidateVisibleAsync(_runtimeContext.FullName))
            .Should()
            .BeTrue($"candidate '{_runtimeContext.FullName}' should appear in the recruitment candidates list after search");
    }

    public async Task OpenCreatedCandidateAsync()
    {
        FrameworkLogger.Info($"Opening candidate details for {_runtimeContext.FullName}.");
        await _candidateListPage.OpenCandidateAsync(_runtimeContext.FullName);
    }
}
