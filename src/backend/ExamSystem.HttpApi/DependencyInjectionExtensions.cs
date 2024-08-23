using ExamSystem.HttpApi.RequestHandlers;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ExamSystem.HttpApi;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection RegisterHttpApiServices(this IServiceCollection services)
    {
        services.TryAddScoped<UpdateProfileHandler>();
        services.TryAddScoped<SignupRequestHandler>();
        services.TryAddScoped<LoginRequestHandler>();
        services.TryAddScoped<ConfirmAccountRequestHandler>();
        services.TryAddScoped<ResetPasswordRequestHandler>();
        services.TryAddScoped<ForgotPasswordRequestHandler>();
        services.TryAddScoped<ResendVerificationCodeRequestHandler>();
        services.TryAddScoped<CreateTagHandler>();
        services.TryAddScoped<UpdateTagHandler>();
        services.TryAddScoped<GetTagsHandler>();
        services.TryAddScoped<DeleteTagHandler>();
        services.TryAddScoped<QuestionCreateRequestHandler>();
        return services;
    }
}
