namespace AutomationFramework.Models;

public class RuntimeCandidateContext
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
    public string Email { get; set; } = string.Empty;
    public string CandidateId { get; set; } = string.Empty;
    public string CurrentStatus { get; set; } = string.Empty;
}
