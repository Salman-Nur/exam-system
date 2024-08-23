using ExamSystem.Application.MembershipFeatures.DataTransferObjects;
using ExamSystem.Application.MembershipFeatures.Enums;
using ExamSystem.Application.MembershipFeatures.Results;
using ExamSystem.Domain.Entities.UnaryAggregateRoots;
using ExamSystem.Infrastructure.Identity.Managers;
using SharpOutcome;

namespace ExamSystem.Infrastructure.Identity.Services;

public interface IMembershipIdentityService
{
    Task<Outcome<(ApplicationIdentityUser appIdentityUser, Member member), MembershipError>> SignupAsync
        (MemberSignupRequest dto);

    Task<string> IssueVerificationTokenAsync(ApplicationIdentityUser user);

    Task<Outcome<(ApplicationIdentityUser appIdentityUser, string token), ForgotPasswordErrorReason>>
        RequestPasswordResetAsync(string email);

    public Task<PasswordResetResult> ResetPasswordAsync(MemberResetPasswordRequest dto);

    public Task<Outcome<ApplicationIdentityUser, CredentialErrorReason>> CanRequestEmailConfirmationAsync
        (MemberResendVerificationCodeRequest dto);
}
