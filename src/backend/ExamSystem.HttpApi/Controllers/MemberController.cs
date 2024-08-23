using DevSkill.AWS.Services.Contracts;
using DevSkill.AWS.Services.Enums;
using ExamSystem.Application.Common.Options;
using ExamSystem.Application.Common.Providers;
using ExamSystem.Application.MembershipFeatures.DataTransferObjects;
using ExamSystem.HttpApi.Others;
using ExamSystem.HttpApi.RequestHandlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ExamSystem.HttpApi.Controllers
{
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("api/v1/[controller]")]
    public class MemberController : ControllerBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IS3BucketService _s3BucketService;
        private readonly AppOptions _appOptions;
        private readonly IGuidProvider _guidProvider;
        private readonly IDateTimeProvider _dateTimeProvider;

        public MemberController(IS3BucketService s3BucketService, IOptions<AppOptions> appOptions,
            IServiceProvider serviceProvider, IGuidProvider guidProvider, IDateTimeProvider dateTimeProvider)
        {
            _s3BucketService = s3BucketService;
            _appOptions = appOptions.Value;

            _serviceProvider = serviceProvider;
            _guidProvider = guidProvider;
            _dateTimeProvider = dateTimeProvider;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            var updateProfileHandler = _serviceProvider.GetRequiredService<UpdateProfileHandler>();
            var memberUpdateDto = await updateProfileHandler.GetMemberUserByEmailAsync(_serviceProvider);

            if (!string.IsNullOrEmpty(memberUpdateDto.ProfilePictureUrl))
            {
                var response = await _s3BucketService.GetFileUrlAsync(_appOptions.S3BucketName,
                    memberUpdateDto.ProfilePictureUrl, _dateTimeProvider.CurrentUtcTime.AddMinutes(5));
                if (response.IsGoodOutcome())
                {
                    response.TryPickGoodOutcome(out var value);
                    memberUpdateDto.ProfilePictureUrl = value;
                }
            }

            return Ok(memberUpdateDto);
        }

        [HttpPut]
		[ValidateAngularXsrfToken]
        [Authorize]
        [ValidationActionFilter<MemberUpdateDTO>]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Put([FromForm] MemberUpdateDTO memberUpdateDto)
        {
            if (memberUpdateDto.ProfilePicture is { Length: > 0 })
            {
                var result = await _s3BucketService.IsBucketExistsAsync(_appOptions.S3BucketName);

                if (result.TryPickBadOutcome(out var err))
                {
                    if (err == S3BadOutcomeTag.BucketNotFound)
                    {
                        await _s3BucketService.CreateBucketAsync(_appOptions.S3BucketName);
                    }
                }

                var fileName = _guidProvider.SortableGuid() + "_" + memberUpdateDto.ProfilePicture.FileName;
                await _s3BucketService.StoreFileAsync(
                    _appOptions.S3BucketName,
                    fileName,
                    memberUpdateDto.ProfilePicture.OpenReadStream(),
                    memberUpdateDto.ProfilePicture.ContentType,
                    HttpContext.RequestAborted
                );
                memberUpdateDto.ProfilePictureUrl = fileName;
            }

            var updateProfileHandler = _serviceProvider.GetRequiredService<UpdateProfileHandler>();
            var message =
                await updateProfileHandler.UpdateMemberInformationAsync(_serviceProvider, memberUpdateDto);

            if (message == null)
            {
                return Ok();
            }

            return BadRequest(message);
        }
    }
}
