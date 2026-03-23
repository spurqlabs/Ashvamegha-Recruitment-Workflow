using Microsoft.Playwright;

namespace AutomationFramework.Utilities;

public static class ScreenshotHelper
{
    public static async Task CaptureAsync(IPage page, string directory, string fileName)
    {
        Directory.CreateDirectory(directory);
        var path = Path.Combine(directory, $"{fileName}_{DateTime.Now:yyyyMMdd_HHmmss}.png");

        await page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = path,
            FullPage = true,
            Timeout = 10000
        });
    }
}
