using DevSkill.AWS.Services.Contracts;
using DevSkill.AWS.Services.Enums;
using ExamSystem.Application.Common.Options;
using ExamSystem.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ExamSystem.Web.Controllers;

public class TestController : Controller
{
    private readonly IS3BucketService _s3BucketService;
    private readonly IServiceProvider _serviceProvider;
    private readonly AppOptions _appOptions;

    public TestController(IS3BucketService s3BucketService, IOptions<AppOptions> appOptions,
        IServiceProvider serviceProvider)
    {
        _s3BucketService = s3BucketService;
        _serviceProvider = serviceProvider;
        _appOptions = appOptions.Value;
    }

    [HttpGet]
    public IActionResult UploadPhoto()
    {
        var viewModel = _serviceProvider.GetRequiredService<ImageUploadViewModel>();
        return View(viewModel);
    }


    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadPhoto(ImageUploadViewModel viewModel)
    {
        if (ModelState.IsValid is false)
        {
            return View();
        }

        var result = await _s3BucketService.IsBucketExistsAsync(_appOptions.S3BucketName);

        if (result.TryPickBadOutcome(out var err))
        {
            if (err == S3BadOutcomeTag.BucketNotFound)
            {
                await _s3BucketService.CreateBucketAsync(_appOptions.S3BucketName);
            }
        }

        await _s3BucketService.StoreFileAsync(
            _appOptions.S3BucketName,
            viewModel.Image.FileName,
            viewModel.Image.OpenReadStream(),
            viewModel.Image.ContentType,
            HttpContext.RequestAborted
        );

        return View();
    }
}
