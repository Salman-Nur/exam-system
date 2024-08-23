namespace ExamSystem.Domain.Entities.Shared;

public interface ITimestamp
{
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}
