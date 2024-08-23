using ExamSystem.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExamSystem.Web.Controllers;

public class EditorController : Controller
{
    [HttpGet]
    public IActionResult Markdown()
    {
        return View();
    }

    [ValidateAntiForgeryToken, HttpPost]
    public IActionResult Markdown(MarkdownEditorViewModel viewModel)
    {
        return View();
    }
}
