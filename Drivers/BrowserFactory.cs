using AutomationFramework.Models;
using Microsoft.Playwright;

namespace AutomationFramework.Drivers;

public class BrowserFactory
{
    public async Task<IBrowser> LaunchAsync(IPlaywright playwright, ConfigModel config)
    {
        var options = new BrowserTypeLaunchOptions
        {
            Headless = config.Headless,
            SlowMo = config.SlowMo
        };

        return config.Browser.ToLowerInvariant() switch
        {
            "firefox" => await playwright.Firefox.LaunchAsync(options),
            "webkit" => await playwright.Webkit.LaunchAsync(options),
            _ => await playwright.Chromium.LaunchAsync(options)
        };
    }
}
