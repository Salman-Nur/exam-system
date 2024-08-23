using Microsoft.AspNetCore.Mvc;

namespace ExamSystem.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class DashboardController : Controller
{
    public ActionResult Index()
    {
        return View();
    }
}
