namespace ExamSystem.Application.MembershipFeatures.Services;

public interface IMembershipEmailSenderService
{
    Task SendVerificationMailAsync(string fullName, string email, string verificationCode);
    Task SendResetPasswordMailAsync(string fullName, string email, string code);
}
