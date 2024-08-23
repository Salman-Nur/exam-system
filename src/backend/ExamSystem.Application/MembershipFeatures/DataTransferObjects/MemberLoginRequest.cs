namespace ExamSystem.Application.MembershipFeatures.DataTransferObjects;

public record MemberLoginRequest(
    string Email,
    string Password,
    bool RememberMe,
    string RecaptchaV3ResponseCode);
