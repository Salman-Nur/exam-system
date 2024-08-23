namespace ExamSystem.Application.MembershipFeatures.DataTransferObjects;

public record MemberResendVerificationCodeRequest(
    string Email,
    string Password,
    string RecaptchaV3ResponseCode);
