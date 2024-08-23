using System.Net;
using ExamSystem.HttpApi.RequestHandlers;
using Microsoft.AspNetCore.Mvc;

namespace ExamSystem.HttpApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class QuestionController(IServiceProvider serviceProvider, ILogger<QuestionController> logger) : ControllerBase
    {
        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            var formCollection = await Request.ReadFormAsync(ct);
            var handler = serviceProvider.GetRequiredService<QuestionCreateRequestHandler>();

            await handler.CreateQuestionAsync(serviceProvider, formCollection, ct);

            return StatusCode(StatusCodes.Status201Created);
        }

    }
}
