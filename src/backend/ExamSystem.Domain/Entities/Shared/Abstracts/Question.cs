using ExamSystem.Domain.Enums;
using ExamSystem.Domain.Misc;

namespace ExamSystem.Domain.Entities.Shared.Abstracts;

public abstract class Question : IEntity<Guid>, ITimestamp
{
    public Guid Id { get; init; }
    public string Title { get; init; }
    public Content? Body { get; set; }
    public byte Score { get; init; }
    public bool IsRequired { get; init; }
    public ushort TimeLimit { get; init; }
    public DifficultyLevel DifficultyLevel { get; init; }
    public string? Hint { get; set; }
    public string? Explanation { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}
