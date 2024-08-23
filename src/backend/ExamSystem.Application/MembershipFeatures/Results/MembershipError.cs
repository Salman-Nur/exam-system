using ExamSystem.Application.MembershipFeatures.Enums;

namespace ExamSystem.Application.MembershipFeatures.Results;

public record MembershipError(MembershipErrorReason Reason, ICollection<KeyValuePair<string, string>> Errors);
