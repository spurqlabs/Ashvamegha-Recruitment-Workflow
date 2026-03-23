using Microsoft.Playwright;

namespace AutomationFramework.Utilities;

public static class WaitHelper
{
    public static async Task WaitForVisibleAsync(ILocator locator, int timeout = 30000)
    {
        await locator.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = timeout
        });
    }

    public static async Task WaitForHiddenAsync(ILocator locator, int timeout = 30000)
    {
        await locator.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Hidden,
            Timeout = timeout
        });
    }
}
