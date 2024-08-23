using ExamSystem.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExamSystem.Web.Controllers;

public class QuestionController : Controller
{
    public IActionResult AllQuestion_2()
    {
        return View();
    }

    public IActionResult SingleQuestion_2()
    {
        return View();
    }

    public IActionResult AllQuestion_3()
    {
        return View();
    }

    public IActionResult Questions_3()
    {
        return View();
    }

    public IActionResult Sample1_1()
    {
        var model = new QuestionPaperModel();
        return View(model);
    }

    public IActionResult Sample2_1()
    {
        var model = new QuestionPaperModel();
        return View(model);
    }
}
