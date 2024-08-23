using ExamSystem.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExamSystem.Web.Controllers;

public class TomSelectController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        var viewModel = new TomSelectViewModel();
        return View(viewModel);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Index(TomSelectViewModel viewModel)
    {
        return View(viewModel);
    }
}
