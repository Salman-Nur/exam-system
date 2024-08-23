namespace ExamSystem.Web.Models;

public class QuestionPaperModel
{
    public string Title { get; set; } = "Class test1";
    public string? Description { get; set; } = "Answers must be submitted during the exam period, or the submit button will be disabled.";
    public int Points { get; set; } = 1;
    public int TotalPoints { get; set; } = 10;
    public int Time { get; set; } = 2;
    public string? ImageUrl { get; set; } = "https://images.pexels.com/photos/1661179/pexels-photo-1661179.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=2";
    public List<string> Questions { get; set; } = new List<string>
    {
        "What is this?",
        "Identify the birds below",
        @"What does the following code return? jwifnfk fjkfrnf fjwnrfjk rfjrki firfmnk fr fkjrnf
            int x = 2;
            int y = 3;
            return x + y;",
        "Correct way to declare a variable in c#",
        "Which of the following cities are in Bangladesh",
        "What is the capital of Bangladesh?"
    };
    public List<List<string>> Options { get; set; } = new List<List<string>>
    {
        new List<string> { "Bird", "Cow", "Horse", "Tiger" },
        new List<string> {  "https://images.pexels.com/photos/1661179/pexels-photo-1661179.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=2",
                            "https://images.pexels.com/photos/1661179/pexels-photo-1661179.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=2",
                            "https://cdn.pixabay.com/photo/2024/04/19/18/05/cubs-8706924_1280.jpg" },

        new List<string> { "2", "5", "6", "8" },
        new List<string> { "int x;", "int y = 2;",
                          @"
        <div class=""text-right"">
            <p class=""mb-0""><span class=""fw-bold text-primary"">Total Points:</span> @Model.TotalPoints</p>
            <p class=""mb-0""><span class=""text-danger fw-bold"">Time:</span> @Model.Time min</p>
        </div>" },
        new List<string> { "Dhaka", "Chittagong", "Mumbai", "Khulna" },
        new List<string> { "Dhaka", "Chittagong", "Rajshahi", "Khulna" }
    };
}
