using ExamSystem.Application.MembershipFeatures.DataTransferObjects;
using ExamSystem.Application.MembershipFeatures.Enums;
using ExamSystem.Infrastructure.Identity.Services;

namespace ExamSystem.HttpApi.RequestHandlers;

public class ResetPasswordRequestHandler
{
    public async Task<bool> ConductResetPasswordAsync(IServiceProvider serviceProvider,
        MemberResetPasswordRequest dto)
    {
        var membershipIdentityService = serviceProvider.GetRequiredService<IMembershipIdentityService>();
        var result = await membershipIdentityService.ResetPasswordAsync(dto);
        return result is PasswordResetResult.Ok;
    }
}
