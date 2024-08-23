using System.Security.Claims;
using ExamSystem.Application.MembershipFeatures.DataTransferObjects;
using ExamSystem.Application.MembershipFeatures.Enums;
using ExamSystem.Application.MembershipFeatures.Results;
using ExamSystem.Application.MembershipFeatures.Services;
using ExamSystem.Infrastructure.Identity.Constants;
using ExamSystem.Infrastructure.Identity.Managers;
using ExamSystem.Infrastructure.Identity.Services;
using SharpOutcome;
using SharpOutcome.Helpers;

namespace ExamSystem.HttpApi.RequestHandlers;

public class SignupRequestHandler
{
    public async Task<ValueOutcome<Successful, MembershipError>> ConductSignupAsync(
        IServiceProvider serviceProvider, MemberSignupRequest dto)
    {
        var membershipIdentityService = serviceProvider.GetRequiredService<IMembershipIdentityService>();

        var result = await membershipIdentityService.SignupAsync(dto);

        if (result.TryPickBadOutcome(out var error, out var userData))
        {
            return error;
        }

        if (userData.appIdentityUser.Email is null)
        {
            return new MembershipError(MembershipErrorReason.Unknown, []);
        }

        var token = await membershipIdentityService.IssueVerificationTokenAsync(userData.appIdentityUser);
        var membershipService = serviceProvider.GetRequiredService<IMembershipService>();
        await membershipService.SendVerificationMailAsync(dto.FullName, userData.appIdentityUser.Email,
            token);

        var appUserManager = serviceProvider.GetRequiredService<ApplicationUserManager>();

        await appUserManager.AddClaimAsync(userData.appIdentityUser,
            new Claim(ClaimTypeConstants.Member, bool.TrueString));

        return new Successful();
    }
}
