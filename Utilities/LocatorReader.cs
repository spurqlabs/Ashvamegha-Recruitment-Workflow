using Newtonsoft.Json.Linq;

namespace AutomationFramework.Utilities;

public class LocatorReader
{
    private readonly Dictionary<string, JObject> _locatorSections;

    public LocatorReader(string locatorDirectoryPath)
    {
        if (!Directory.Exists(locatorDirectoryPath))
        {
            throw new DirectoryNotFoundException($"Locator directory not found: {locatorDirectoryPath}");
        }

        _locatorSections = Directory
            .GetFiles(locatorDirectoryPath, "*.json", SearchOption.TopDirectoryOnly)
            .ToDictionary(
                filePath => Path.GetFileNameWithoutExtension(filePath) ?? throw new InvalidOperationException($"Unable to resolve locator file name for: {filePath}"),
                filePath => JObject.Parse(File.ReadAllText(filePath)),
                StringComparer.OrdinalIgnoreCase);

        if (_locatorSections.Count == 0)
        {
            throw new InvalidOperationException($"No locator JSON files were found in: {locatorDirectoryPath}");
        }
    }

    public string Get(string section, string key)
    {
        if (!_locatorSections.TryGetValue(section, out var sectionObject))
        {
            throw new KeyNotFoundException($"Locator section file not found for '{section}'.");
        }

        var value = sectionObject[key]?.ToString();

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new KeyNotFoundException($"Locator not found for section '{section}' and key '{key}'.");
        }

        return value;
    }
}
