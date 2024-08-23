using ExamSystem.Domain.Entities.Shared;
using ExamSystem.Domain.Enums;

namespace ExamSystem.Domain.Entities.UnaryAggregateRoots;

public class Member : IEntity<Guid>, ITimestamp
{
    public required Guid Id { get; init; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public string? ProfilePictureUri { get; set; }
    public required UserStatus Status { get; set; }
    public required DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}
