using ExamSystem.Domain.Entities.Shared;
using ExamSystem.Domain.Entities.Shared.Abstracts;

namespace ExamSystem.Domain.Misc;

public class Content : IEntity<Guid>
{
    public required Guid Id { get; init; }
    public required HashSet<ContentElement> Elements { get; set; }
}
