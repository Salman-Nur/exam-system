namespace ExamSystem.Application.MembershipFeatures.DataTransferObjects;

public record MemberResetPasswordRequest(
    string Email,
    string Password,
    string Code,
    string RecaptchaV3ResponseCode);
