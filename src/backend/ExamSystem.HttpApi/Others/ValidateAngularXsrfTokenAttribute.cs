using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ExamSystem.HttpApi.Others;

[AttributeUsage(AttributeTargets.Method)]
public class ValidateAngularXsrfTokenAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var antiForgery = (IAntiforgery)context
            .HttpContext.RequestServices
            .GetRequiredService(typeof(IAntiforgery));
    
        if (await antiForgery.IsRequestValidAsync(context.HttpContext) is false)
        {
            context.Result = context.MakeResponse(StatusCodes.Status403Forbidden, "xsrf error");
            return;
        }

        await next();
    }
}
