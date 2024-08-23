namespace ExamSystem.Application.MembershipFeatures.Enums;

public enum PasswordResetResult : byte
{
    UserNotFound = 1,
    ProfileNotConfirmed,
    SameAsOldPassword,
    InvalidToken,
    Ok
}
