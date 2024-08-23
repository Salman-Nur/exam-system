using ExamSystem.Application.MembershipFeatures.Services;
using ExamSystem.Infrastructure.Identity.Services;

namespace ExamSystem.HttpApi.RequestHandlers;

public class ForgotPasswordRequestHandler
{
    public async Task<bool> ConductForgotPasswordRequestAsync(IServiceProvider serviceProvider, string email,
        CancellationToken ct)
    {
        var membershipIdentityService = serviceProvider.GetRequiredService<IMembershipIdentityService>();
        var result = await membershipIdentityService.RequestPasswordResetAsync(email);

        if (result.TryPickGoodOutcome(out var data) is false)
        {
            return false;
        }

        var membershipService = serviceProvider.GetRequiredService<IMembershipService>();
        var member = await membershipService.GetOneAsync(data.appIdentityUser.Id, ct);
        if (member is null || data.appIdentityUser.Email is null)
        {
            return false;
        }

        await membershipService.SendResetPasswordMailAsync(
            member.FullName,
            data.appIdentityUser.Email,
            data.token
        );

        return true;
    }
}
