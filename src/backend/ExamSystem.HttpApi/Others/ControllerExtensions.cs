using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace ExamSystem.HttpApi.Others;

public static class ControllerExtensions
{
    public static IActionResult MakeResponse(this ActionContext context, int code, object? data = null)
    {
        return new JsonResult(data) { StatusCode = code };
    }

    public static IActionResult MakeValidationErrorResponse(this ActionContext context)
    {
        var errors = context.ModelState
        .Where(e => e.Value is { Errors.Count: > 0 })
        .Select(e => new
        {
            Field = JsonNamingPolicy.CamelCase.ConvertName(e.Key),
            Errors = e.Value?.Errors.Select(er => er.ErrorMessage)
        });

        return context.MakeResponse(StatusCodes.Status400BadRequest, errors);
    }
}
