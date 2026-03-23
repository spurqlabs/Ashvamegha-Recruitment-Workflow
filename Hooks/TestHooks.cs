using AutomationFramework.Drivers;
using AutomationFramework.Models;
using AutomationFramework.Utilities;
using Newtonsoft.Json.Linq;
using Reqnroll;

namespace AutomationFramework.Hooks;

[Binding]
public sealed class TestHooks
{
    private readonly ScenarioContext _scenarioContext;
    private PlaywrightDriver? _driver;

    public TestHooks(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [BeforeScenario(Order = 0)]
    public async Task BeforeScenarioAsync()
    {
        var config = new ConfigReader(PathHelper.ResolveFromProject("TestData/config.json")).GetConfig();
        var locatorReader = new LocatorReader(PathHelper.ResolveFromProject("Locator"));
        var scenarioDataFile = ResolveScenarioDataFile(_scenarioContext.ScenarioInfo.Title);
        var allScenarioData = new TestDataReader(PathHelper.ResolveFromProject(scenarioDataFile)).GetTestData<JObject>();
        var scenarioToken = allScenarioData[_scenarioContext.ScenarioInfo.Title];

        if (scenarioToken is null)
        {
            throw new KeyNotFoundException(
                $"No test data mapping found for scenario '{_scenarioContext.ScenarioInfo.Title}' in '{scenarioDataFile}'.");
        }

        var scenarioData = scenarioToken as JObject
            ?? throw new InvalidOperationException(
                $"Scenario data for '{_scenarioContext.ScenarioInfo.Title}' in '{scenarioDataFile}' must be a JSON object.");

        var candidateData = scenarioData.ToObject<CandidateDataModel>() ?? new CandidateDataModel();

        FrameworkLogger.Info($"Resolved scenario data from {scenarioDataFile} for scenario '{_scenarioContext.ScenarioInfo.Title}'.");

        _driver = new PlaywrightDriver(config);
        await _driver.InitializeAsync();

        _scenarioContext.Set(config);
        _scenarioContext.Set(locatorReader);
        _scenarioContext.Set(candidateData);
        _scenarioContext.Set(scenarioData);
        _scenarioContext.Set(new RuntimeCandidateContext());

        if (_driver.Page is null)
        {
            throw new InvalidOperationException("Playwright page was not initialized.");
        }

        _scenarioContext.Set(_driver);
        _scenarioContext.Set(_driver.Page);
    }

    [AfterScenario(Order = 100)]
    public async Task AfterScenarioAsync()
    {
        if (_driver is null)
        {
            return;
        }

        ConfigModel? config = null;

        try
        {
            config = _scenarioContext.Get<ConfigModel>();
        }
        catch (KeyNotFoundException)
        {
            // BeforeScenario may have failed before config was stored.
        }

        if (_driver.Page is not null &&
            _scenarioContext.TestError is not null &&
            config is not null &&
            config.ScreenshotOnFailure)
        {
            await ScreenshotHelper.CaptureAsync(
                _driver.Page,
                PathHelper.ResolveFromProject(config.ScreenshotPath),
                _scenarioContext.ScenarioInfo.Title.Replace(" ", "_"));
        }

        await _driver.DisposeAsync();
    }

    private static string ResolveScenarioDataFile(string scenarioTitle)
    {
        return scenarioTitle switch
        {
            "Add a new timesheet entry and verify total hours" => "TestData/timesheetData.json",
            "Apply leave from JSON data and verify it is pending approval" => "TestData/leaveData.json",
            _ => "TestData/candidateData.json"
        };
    }
}
