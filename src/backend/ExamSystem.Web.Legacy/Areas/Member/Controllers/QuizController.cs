using Microsoft.AspNetCore.Mvc;

namespace ExamSystem.Web.Areas.Member.Controllers;

[Area("Member")]
public class QuizController : Controller
{
    public IActionResult Start()
    {
        return View();
    }

    public IActionResult Result()
    {
        return View();
    }
}
