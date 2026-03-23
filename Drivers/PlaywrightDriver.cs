using AutomationFramework.Models;
using Microsoft.Playwright;

namespace AutomationFramework.Drivers;

public class PlaywrightDriver : IAsyncDisposable
{
    private readonly ConfigModel _config;
    private readonly BrowserFactory _browserFactory = new();

    private IPlaywright? _playwright;
    private IBrowser? _browser;

    public IBrowserContext? BrowserContext { get; private set; }
    public IPage? Page { get; private set; }

    public PlaywrightDriver(ConfigModel config)
    {
        _config = config;
    }

    public async Task InitializeAsync()
    {
        _playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        _browser = await _browserFactory.LaunchAsync(_playwright, _config);

        BrowserContext = await _browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1440, Height = 900 }
        });

        Page = await BrowserContext.NewPageAsync();
        Page.SetDefaultTimeout(_config.DefaultTimeout);
    }

    public async ValueTask DisposeAsync()
    {
        if (Page is not null)
        {
            await Page.CloseAsync();
        }

        if (BrowserContext is not null)
        {
            await BrowserContext.CloseAsync();
        }

        if (_browser is not null)
        {
            await _browser.CloseAsync();
        }

        _playwright?.Dispose();
    }
}
