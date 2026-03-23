using AutomationFramework.Models;
using AutomationFramework.Utilities;
using Microsoft.Playwright;

namespace AutomationFramework.Pages;

public class CandidateListPage : BasePage
{
    public CandidateListPage(IPage page, LocatorReader locatorReader) : base(page, locatorReader)
    {
    }

    public async Task SearchByCandidateNameAsync(string candidateName)
    {
        var firstName = candidateName.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];
        var candidateNameInput = Locator("CandidateListPage", "candidateNameSearchInput");
        var suggestionOptions = Locator("CandidateListPage", "candidateNameOptions");

        await candidateNameInput.ClickAsync();
        await candidateNameInput.FillAsync(string.Empty);
        await candidateNameInput.TypeAsync(firstName);

        await suggestionOptions.First.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 15000
        });

        await suggestionOptions.First.ClickAsync();
        await ClickAsync("CandidateListPage", "searchButton");

        await Locator("CandidateListPage", "candidateRows").First.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 30000
        });
    }

    public async Task<bool> IsCandidateVisibleAsync(string candidateName)
    {
        var row = Locator("CandidateListPage", "candidateRows").Filter(new() { HasTextString = candidateName }).First;
        return await row.IsVisibleAsync();
    }

    public async Task OpenCandidateAsync(string candidateName)
    {
        var row = Locator("CandidateListPage", "candidateRows").Filter(new() { HasTextString = candidateName }).First;
        await row.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        await row.ClickAsync();
    }

    public async Task UpdateStatusAsync(string status, CandidateInfo candidate)
    {
        if (status.Equals("Shortlisted", StringComparison.OrdinalIgnoreCase) ||
            status.Equals("Shortlist", StringComparison.OrdinalIgnoreCase))
        {
            await ClickAsync("CandidateListPage", "shortlistButton");
        }
        else
        {
            await ClickAsync("CandidateListPage", "scheduleInterviewButton");
            await FillInterviewScheduleAsync(candidate);
        }

        await ClickAsync("CandidateListPage", "saveStatusButton");
    }

    private async Task FillInterviewScheduleAsync(CandidateInfo candidate)
    {
        var interviewTitleInput = Page.Locator("//label[contains(.,'Interview Title')]/ancestor::div[contains(@class,'oxd-input-group')]//input").First;
        var interviewerInput = Page.Locator("//label[contains(.,'Interviewer')]/ancestor::div[contains(@class,'oxd-input-group')]//input").First;
        var dateInput = Page.Locator("//label[contains(.,'Date')]/ancestor::div[contains(@class,'oxd-input-group')]//input").First;
        var timeInput = Page.Locator("//label[contains(.,'Time')]/ancestor::div[contains(@class,'oxd-input-group')]//input").First;
        var notesInput = Page.Locator("//label[contains(.,'Notes')]/ancestor::div[contains(@class,'oxd-input-group')]//textarea").First;

        await interviewTitleInput.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });
        await interviewTitleInput.FillAsync(candidate.InterviewTitle);

        await interviewerInput.FillAsync(candidate.Interviewer);
        await interviewerInput.PressAsync("ArrowDown");
        await interviewerInput.PressAsync("Enter");

        await dateInput.FillAsync(candidate.InterviewDate);
        await timeInput.FillAsync(candidate.InterviewTime);

        if (!string.IsNullOrWhiteSpace(candidate.InterviewNotes) && await notesInput.IsVisibleAsync())
        {
            await notesInput.FillAsync(candidate.InterviewNotes);
        }
    }

    public async Task<bool> IsStatusUpdatedAsync(string status)
    {
        var statusText = Page.GetByText(status, new PageGetByTextOptions { Exact = false }).First;

        try
        {
            await statusText.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 10000
            });

            return await statusText.IsVisibleAsync();
        }
        catch (TimeoutException)
        {
        }

        var expectedNextAction = status.Trim().ToLowerInvariant() switch
        {
            "shortlisted" or "shortlist" => "Schedule Interview",
            "interview scheduled" => "Schedule Interview",
            _ => throw new ArgumentOutOfRangeException(nameof(status), $"Unsupported status: {status}")
        };

        var nextActionButton = Page.Locator($"button:has-text('{expectedNextAction}')").First;
        await nextActionButton.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Hidden,
            Timeout = 30000
        });

        return !await nextActionButton.IsVisibleAsync();
    }
}
