using ExamSystem.Domain.Entities.Shared;

namespace ExamSystem.Domain.Entities.UnaryAggregateRoots;

public class SystemClaim : IEntity<Guid>
{
    public required Guid Id { get; init; }
    public required string Name { get; set; }
    public string? Description { get; set; }
}
