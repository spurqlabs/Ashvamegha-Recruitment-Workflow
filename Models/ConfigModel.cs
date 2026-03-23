namespace AutomationFramework.Models;

public class ConfigModel
{
    public string ApplicationUrl { get; set; } = string.Empty;
    public string Browser { get; set; } = "chromium";
    public bool Headless { get; set; }
    public int SlowMo { get; set; }
    public float DefaultTimeout { get; set; }
    public float ExpectTimeout { get; set; }
    public bool ScreenshotOnFailure { get; set; }
    public string ScreenshotPath { get; set; } = "Screenshots";
    public string ReportPath { get; set; } = "Reports";
    public string BaseTestDataPath { get; set; } = "TestData";
    public string ResumePath { get; set; } = "Resources/resumes/sample_resume.pdf";
}
