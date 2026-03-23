namespace AutomationFramework.Models;

public class LoginData
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class CandidateInfo
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Vacancy { get; set; } = string.Empty;
    public string Keywords { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string ResumePath { get; set; } = string.Empty;
    public string StatusToUpdate { get; set; } = string.Empty;
    public string SecondaryStatusToUpdate { get; set; } = string.Empty;
    public string InterviewTitle { get; set; } = string.Empty;
    public string Interviewer { get; set; } = string.Empty;
    public string InterviewDate { get; set; } = string.Empty;
    public string InterviewTime { get; set; } = string.Empty;
    public string InterviewNotes { get; set; } = string.Empty;
}

public class CandidateDataModel
{
    public LoginData Login { get; set; } = new();
    public CandidateInfo Candidate { get; set; } = new();
}
