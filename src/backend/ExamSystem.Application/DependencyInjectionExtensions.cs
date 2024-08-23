using ExamSystem.Application.MembershipFeatures.Services;
using ExamSystem.Application.QuestionManagementFeatures.Services;
using ExamSystem.Application.TagFeature.Service;
using FluentValidation;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ExamSystem.Application;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
    {
        services.TryAddScoped<IMembershipService, MembershipService>();
        services.TryAddScoped<IMembershipEmailSenderService, MembershipEmailSenderService>();

        services.TryAddScoped<IQuestionManagementService, QuestionManagementService>();
        services.TryAddScoped<ITagManagementService, TagManagementService>();
        services.AddValidatorsFromAssemblyContaining(typeof(IApplicationMarker));
        return services;
    }
}
