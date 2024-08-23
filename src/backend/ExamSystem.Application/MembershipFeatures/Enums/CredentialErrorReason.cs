namespace ExamSystem.Application.MembershipFeatures.Enums;

public enum CredentialErrorReason : byte
{
    UserNotFound = 1,
    PasswordNotMatched,
    ProfileAlreadyConfirmed
}
