using Microsoft.AspNetCore.Mvc;

namespace ExamSystem.Web.Controllers;

public class FileController : Controller
{
    private readonly IWebHostEnvironment _environment;

    public FileController(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file is not { Length: > 0 })
        {
            return RedirectToAction("Index");
        }

        var uploads = Path.Combine(_environment.ContentRootPath, "..", "..", "uploads");
        if (!Directory.Exists(uploads))
        {
            Directory.CreateDirectory(uploads);
        }

        var filePath = Path.Combine(uploads, file.FileName);
        await using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }
        return Ok(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return BadRequest("Filename is required.");
        }

        var uploads = Path.Combine(_environment.ContentRootPath, "..", "..", "uploads");
        var filePath = Path.Combine(uploads, fileName);

        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
            return Ok(new { success = true });
        }
        else
        {
            return NotFound();
        }
    }

    public IActionResult PhotoView()
    {
        return View();
    }

    public IActionResult VideoView()
    {
        return View();
    }
}
