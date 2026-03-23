using AutomationFramework.Models;
using AutomationFramework.Utilities;
using Microsoft.Playwright;

namespace AutomationFramework.Pages;

public class AddCandidatePage : BasePage
{
    public AddCandidatePage(IPage page, LocatorReader locatorReader) : base(page, locatorReader)
    {
    }

    public async Task ClickAddCandidateAsync()
    {
        await ClickAsync("AddCandidatePage", "addButton");
        await Locator("AddCandidatePage", "firstNameInput").WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 30000
        });
    }

    public async Task AddCandidateAsync(CandidateInfo candidate)
    {
        await FillAsync("AddCandidatePage", "firstNameInput", candidate.FirstName);
        await FillAsync("AddCandidatePage", "lastNameInput", candidate.LastName);

        var vacancyDropdown = Locator("AddCandidatePage", "vacancyDropdown").First;
        if (await vacancyDropdown.IsVisibleAsync())
        {
            await vacancyDropdown.ClickAsync();
            var option = Locator("AddCandidatePage", "vacancyOptions")
                .Filter(new() { HasTextString = candidate.Vacancy })
                .First;
            await option.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 30000
            });
            await option.ClickAsync();
        }
        else
        {
            FrameworkLogger.Warn("Vacancy dropdown was not visible on the Add Candidate form. Continuing without vacancy selection.");
        }

        await FillAsync("AddCandidatePage", "emailInput", candidate.Email);
        await FillAsync("AddCandidatePage", "contactInput", candidate.Phone);
        await FillAsync("AddCandidatePage", "keywordsInput", candidate.Keywords);
        await FillAsync("AddCandidatePage", "notesInput", candidate.Notes);
        await UploadFileAsync("AddCandidatePage", "resumeUploadInput", candidate.ResumePath);
    }

    public async Task SaveAsync()
    {
        await ClickAsync("AddCandidatePage", "saveButton");
    }

    public async Task<string> GetSuccessMessageAsync()
    {
        return await GetTextAsync("AddCandidatePage", "successToast");
    }

    public async Task WaitForCandidateDetailsPageToLoadAsync()
    {
        await Page.WaitForURLAsync("**/recruitment/addCandidate/*", new PageWaitForURLOptions
        {
            Timeout = 30000
        });

        await Locator("AddCandidatePage", "applicationStageHeader").WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 30000
        });

        await Locator("AddCandidatePage", "candidateProfileHeader").WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 30000
        });
    }

    public async Task<string> GetCandidateIdAsync()
    {
        var locator = Locator("AddCandidatePage", "candidateIdValue");
        await locator.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        return await locator.InputValueAsync();
    }
}
