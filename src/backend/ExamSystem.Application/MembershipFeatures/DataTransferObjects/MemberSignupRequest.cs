namespace ExamSystem.Application.MembershipFeatures.DataTransferObjects;

public record MemberSignupRequest(
    string FullName,
    string Email,
    string Password,
    string ConfirmPassword,
    string RecaptchaV3ResponseCode);
