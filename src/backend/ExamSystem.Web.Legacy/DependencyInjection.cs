using ExamSystem.Web.Areas.Admin.Models;
using ExamSystem.Web.Models;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ExamSystem.Web;

public static class DependencyInjection
{
    public static IServiceCollection RegisterWebServices(this IServiceCollection services)
    {
        services.TryAddScoped<MemberModel>();
        services.TryAddScoped<SingleQuestionViewModel>();
        services.TryAddScoped<ImageUploadViewModel>();
        return services;
    }
}
