using ExamSystem.Web.Areas.Admin.Models;
using ExamSystem.Web.Models.Utilities.TabulatorUtilities;
using Microsoft.AspNetCore.Mvc;

namespace ExamSystem.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MemberController(IServiceProvider serviceProvider, ILogger<MemberController> logger) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetMembers([FromBody] TabulatorRequest request)
        {
            var model = serviceProvider.GetRequiredService<MemberModel>();
            var tabulatorResponse = await model.GetMembersAsync(request);
            return Ok(tabulatorResponse);
        }

        public IActionResult UpdateProfile()
        {
            var model = new ProfileUpdateModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult UpdateProfile(ProfileUpdateModel model)
        {
            if (ModelState.IsValid)
            {
                return View(model);
            }

            return View(model);
        }

        public IActionResult ViewMember(Guid id)
        {
            return View();
        }

        [HttpPost]
        public IActionResult DeleteMember(Guid id)
        {
            var model = serviceProvider.GetRequiredService<MemberModel>();
            var result = true;

            if (result)
            {
                return Json(new { success = true, message = "Member deleted successfully." });
            }
            else
            {
                return Json(new { success = false, message = "An error occurred while deleting the member." });
            }
        }
    }
}
