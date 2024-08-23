namespace ExamSystem.Application.MembershipFeatures.DataTransferObjects;

public record MemberConfirmAccountRequest(string Email, string Code, string RecaptchaV3ResponseCode);
