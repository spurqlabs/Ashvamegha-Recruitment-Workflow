namespace AutomationFramework.Utilities;

public static class PathHelper
{
    public static string ResolveFromProject(string relativePath)
    {
        return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, relativePath));
    }
}
