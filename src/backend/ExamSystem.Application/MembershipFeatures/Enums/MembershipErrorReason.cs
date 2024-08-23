namespace ExamSystem.Application.MembershipFeatures.Enums;

public enum MembershipErrorReason : byte
{
    DuplicateEmail = 1,
    NotFound,
    Others,
    Unknown
}
