namespace ExamSystem.Web.Models.Utilities.TabulatorUtilities;

public sealed record TabulatorOrder
{
    public string Field { get; set; }
    public string Dir { get; set; } = "asc";
}
