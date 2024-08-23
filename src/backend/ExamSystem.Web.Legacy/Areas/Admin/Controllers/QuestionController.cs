using ExamSystem.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExamSystem.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class QuestionController : Controller
{
    private readonly IServiceProvider _serviceProvider;

    public QuestionController(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IActionResult Create()
    {
        var viewModel = _serviceProvider.GetRequiredService<SingleQuestionViewModel>();
        return View(viewModel);
    }
}
