namespace ExamSystem.Domain.Entities.Shared.Abstracts;

public abstract class ContentElement : IEntity<Guid>
{
    public required uint Serial { get; set; }
    public required Guid Id { get; init; }
}
