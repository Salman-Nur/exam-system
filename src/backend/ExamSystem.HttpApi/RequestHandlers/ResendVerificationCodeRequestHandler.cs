using ExamSystem.Application.MembershipFeatures.DataTransferObjects;
using ExamSystem.Application.MembershipFeatures.Enums;
using ExamSystem.Application.MembershipFeatures.Services;
using ExamSystem.Infrastructure.Identity.Managers;
using ExamSystem.Infrastructure.Identity.Services;
using SharpOutcome;
using SharpOutcome.Helpers;

namespace ExamSystem.HttpApi.RequestHandlers;

public class ResendVerificationCodeRequestHandler
{
    public async Task<ValueOutcome<Successful, string>> ConductResendVerificationCodeAsync
        (IServiceProvider serviceProvider, MemberResendVerificationCodeRequest dto, CancellationToken ct)
    {
        const string genericErrorMsg = "Something went wrong";
        var membershipIdentityService = serviceProvider.GetRequiredService<IMembershipIdentityService>();
        var result = await membershipIdentityService.CanRequestEmailConfirmationAsync(dto);

        if (result.TryPickBadOutcome(out var error, out var appIdentityUser))
        {
            return error == CredentialErrorReason.ProfileAlreadyConfirmed
                ? "Account already confirmed"
                : "Invalid attempt";
        }

        if (appIdentityUser.Email is null)
        {
            return genericErrorMsg;
        }

        var membershipService = serviceProvider.GetRequiredService<IMembershipService>();
        var member = await membershipService.GetOneAsync(appIdentityUser.Id, ct);
        if (member is null)
        {
            return genericErrorMsg;
        }

        var applicationUserManager = serviceProvider.GetRequiredService<ApplicationUserManager>();
        await applicationUserManager.UpdateSecurityStampAsync(appIdentityUser);

        var token = await membershipIdentityService.IssueVerificationTokenAsync(appIdentityUser);
        await membershipService.SendVerificationMailAsync(member.FullName, appIdentityUser.Email, token);
        return new Successful();
    }
}
