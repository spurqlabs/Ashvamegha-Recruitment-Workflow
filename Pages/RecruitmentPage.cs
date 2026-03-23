using AutomationFramework.Utilities;
using Microsoft.Playwright;

namespace AutomationFramework.Pages;

public class RecruitmentPage : BasePage
{
    public RecruitmentPage(IPage page, LocatorReader locatorReader) : base(page, locatorReader)
    {
    }

    public async Task NavigateToCandidatesAsync()
    {
        await ClickAsync("RecruitmentPage", "recruitmentMenu");
        await ClickAsync("RecruitmentPage", "candidatesTab");
        await Locator("RecruitmentPage", "pageHeader").WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible
        });
    }
}
