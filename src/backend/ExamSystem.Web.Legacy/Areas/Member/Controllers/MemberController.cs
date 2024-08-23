using Microsoft.AspNetCore.Mvc;

namespace ExamSystem.Web.Areas.Member.Controllers;

[Area("Member")]
public class MemberController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
    public IActionResult UpdateProfile()
    {
        return View();
    }
}
