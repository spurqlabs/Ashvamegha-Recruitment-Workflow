using AutomationFramework.Models;

namespace AutomationFramework.Utilities;

public class ConfigReader
{
    private readonly string _configPath;
    private ConfigModel? _config;

    public ConfigReader(string configPath)
    {
        _configPath = configPath;
    }

    public ConfigModel GetConfig()
    {
        _config ??= JsonFileReader.Read<ConfigModel>(_configPath);
        return _config;
    }
}
