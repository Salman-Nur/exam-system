namespace ExamSystem.Web.Areas.Admin.Models;

public class ProfileUpdateModel
{
    public string Email { get; set; } = "salman.installer@gmail.com";
    public string FullName { get; set; } = "MD. SALMAN NUR";
    public List<string> Country { get; set; } = new List<string> { "Bangladesh", "India", "Pakistan", "Nepal" };
    public string PhoneNumber { get; set; } = "1608159011";
    public string Password { get; set; } = "123456";
    public string ConfirmPassword { get; set; } = "123456";
    public List<string> AllClaims { get; set; } = new List<string> { "CreateQuestion", "ViewQuestion", "CreateExam", "ViewExam", "AttendExam" };
    public List<string> UserClaims { get; set; } = new List<string> { "ViewExam", "AttendExam" };
    public Dictionary<string, string> ClaimDescriptions { get; set; } = new Dictionary<string, string>
        {
            { "CreateQuestion", "Permission to create new questions" },
            { "ViewQuestion", "Permission to view existing questions" },
            { "CreateExam", "Permission to create new exams" },
            { "ViewExam", "Permission to view existing exams" },
            { "AttendExam", "Permission to attend exams" }
        };
}
