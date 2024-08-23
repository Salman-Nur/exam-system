using ExamSystem.Application.Common.Contracts;
using ExamSystem.Application.Common.Providers;
using ExamSystem.Application.Common.Services;
using ExamSystem.Domain.Repositories;
using ExamSystem.Infrastructure.Identity.Services;
using ExamSystem.Infrastructure.Persistence;
using ExamSystem.Infrastructure.Persistence.Repositories;
using ExamSystem.Infrastructure.Providers;
using ExamSystem.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ExamSystem.Infrastructure;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection RegisterInfrastructureServices(this IServiceCollection services)
    {
        services.TryAddSingleton<IGuidProvider, GuidProvider>();
        services.TryAddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.TryAddSingleton<IJwtProvider, JwtProvider>();

        services.TryAddScoped<IMembershipIdentityService, MembershipIdentityService>();
        services.TryAddScoped<IApplicationUnitOfWork, ApplicationUnitOfWork>();
        services.TryAddScoped<IMemberRepository, MemberRepository>();
        services.TryAddScoped<ITagRepository, TagRepository>();
        services.TryAddScoped<IEmailService, HtmlEmailService>();
        services.TryAddScoped<IQuestionRepository, QuestionRepository>();
        return services;
    }
}
