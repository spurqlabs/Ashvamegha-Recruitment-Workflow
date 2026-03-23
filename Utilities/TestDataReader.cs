namespace AutomationFramework.Utilities;

public class TestDataReader
{
    private readonly string _dataPath;

    public TestDataReader(string dataPath)
    {
        _dataPath = dataPath;
    }

    public T GetTestData<T>()
    {
        return JsonFileReader.Read<T>(_dataPath);
    }
}
