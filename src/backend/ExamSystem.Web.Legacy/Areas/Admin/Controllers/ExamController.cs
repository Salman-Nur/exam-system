using ExamSystem.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExamSystem.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ExamController : Controller
    {
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(ExamCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public IActionResult DemoQuestions(string tagName)
        {
            List<object> questions = [
                new { Id = 1, Text = "Question 1Question 1Question 1Question 1Question 1Question 1Question 1Question 1Question" },
                new { Id = 2, Text = "Question 2" },
                new { Id = 3, Text = "Question 3" },
                new { Id = 4, Text = "Question 4" },
                new { Id = 5, Text = "Question 5" }
            ];
            return Json(questions);
        }

        [HttpGet]
        public IActionResult Question(int id)
        {
            List<string> options = ["Option1", "Option2", "Option3", "Option4"];
            var question = new { id = 1, title = "What is your name?", options };
            return Json(question);
        }
    }
}
