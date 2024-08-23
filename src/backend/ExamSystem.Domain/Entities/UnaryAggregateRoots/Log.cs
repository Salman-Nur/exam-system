using ExamSystem.Domain.Entities.Shared;

namespace ExamSystem.Domain.Entities.UnaryAggregateRoots;

public class Log : IEntity<long>
{
    public required long Id { get; init; }
    public string? Message { get; set; }
    public string? MessageTemplate { get; set; }
    public string? Level { get; set; }
    public required DateTime TimeStamp { get; set; }
    public string? Exception { get; set; }
    public string? Properties { get; set; }
}
