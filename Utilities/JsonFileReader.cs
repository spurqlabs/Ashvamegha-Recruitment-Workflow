using AutomationFramework.Models;
using Newtonsoft.Json;

namespace AutomationFramework.Utilities;

public static class JsonFileReader
{
    public static T Read<T>(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"JSON file not found: {path}");
        }

        var content = File.ReadAllText(path);
        var data = JsonConvert.DeserializeObject<T>(content);

        return data ?? throw new InvalidOperationException($"Unable to deserialize JSON file: {path}");
    }
}
