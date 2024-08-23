using ExamSystem.Application.MembershipFeatures.DataTransferObjects;
using ExamSystem.Application.TagFeature.DataTransferObjects;
using ExamSystem.Application.TagFeature.Service;
using ExamSystem.HttpApi.Others;
using ExamSystem.HttpApi.RequestHandlers;
using ExamSystem.Infrastructure.Identity.Managers;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Sprache;


namespace ExamSystem.HttpApi.Controllers
{
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("api/v1/[controller]")]
    public class TagController : ControllerBase
    {
        private readonly ILogger<MemberController> _logger;
        private readonly IServiceProvider _serviceProvider;

        public TagController(ILogger<MemberController> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int pageIndex = 1, int pageSize = 10)
        {
            var getTagsHandler = _serviceProvider.GetRequiredService<GetTagsHandler>();
            var tagListDto = await getTagsHandler.GetTagsAsync(_serviceProvider, pageIndex, pageSize);
            var response = new
            {
                data = tagListDto.data,
                total = tagListDto.total,
                totalDisplay = tagListDto.totalDisplay
            };

            return Ok(response);
        }

        [HttpGet("all")]
        public async Task<IActionResult> Getall()
        {
            var getTagHandler = _serviceProvider.GetRequiredService<GetTagsHandler>();
            var tagDto = await getTagHandler.GetAllTags(_serviceProvider);
            return Ok(tagDto);
        }

        [HttpPost]
        [ValidateAngularXsrfToken]
        [ValidationActionFilter<TagCreateDTO>]
        public async Task<IActionResult> Post(TagCreateDTO tagCreateDTO)
        {
            var result = await CreateTagHandler.CreateTagAsync(_serviceProvider, tagCreateDTO);

            if (result.TryPickGoodOutcome(out var id))
            {
                return Ok(id);
            }

            _logger.LogWarning("Tag creation error, Duplicate found");
            return BadRequest("Tag already exists !!");
        }

        [HttpPut("{id:guid}")]
        [ValidateAngularXsrfToken]
        [ValidationActionFilter<TagUpdateDTO>]
        public async Task<IActionResult> Put(Guid id, TagUpdateDTO tagUpdateDTO)
        {
            var tagUpdateHandler = _serviceProvider.GetRequiredService<UpdateTagHandler>();
            var response = await tagUpdateHandler.GetTagByIdAsync(_serviceProvider, id);
            if (response.IsBadOutcome())
            {
                response.TryPickBadOutcome(out var errorMessage);
                return BadRequest(errorMessage);
            }
            response.TryPickGoodOutcome(out var tagDTO);
            string? message = await tagUpdateHandler.UpdateTagAsync(_serviceProvider, tagUpdateDTO, tagDTO);

            if (message == null)
            {
                return Ok();
            }
            else
            {
                _logger.LogWarning("Tag Update error: {message}", message);
                return BadRequest(message);
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id) {
            var deleteTagHandler = _serviceProvider.GetRequiredService<DeleteTagHandler>();
            await deleteTagHandler.RemoveTagByIdAsync(_serviceProvider ,id);
            return Ok();
        }
    }
}
