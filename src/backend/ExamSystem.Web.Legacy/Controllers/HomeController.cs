using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
namespace ExamSystem.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(int id)
    {
        ViewData["IsException"] = true;
        ViewData["ErrorMessage"] = ReasonPhrases.GetReasonPhrase(id);
        ViewData["ErrorCode"] = id;
        return View("StatusCodeError");
    }
}
