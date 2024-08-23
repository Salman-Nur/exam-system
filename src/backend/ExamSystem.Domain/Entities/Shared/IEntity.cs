namespace ExamSystem.Domain.Entities.Shared;

public interface IEntity<TKey>
    where TKey : IEquatable<TKey>, IComparable
{
    public TKey Id { get; init; }
}
