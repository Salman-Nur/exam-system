using Microsoft.AspNetCore.Mvc;

namespace ExamSystem.Web.Controllers;

public class SweetAlert2Controller : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}
