namespace ExamSystem.Application.MembershipFeatures.DataTransferObjects;

public record MemberForgotPasswordRequest(string Email, string RecaptchaV3ResponseCode);
