namespace ExamSystem.Domain.Misc;

public record PagedData<T>(ICollection<T> Payload, int TotalCount);
