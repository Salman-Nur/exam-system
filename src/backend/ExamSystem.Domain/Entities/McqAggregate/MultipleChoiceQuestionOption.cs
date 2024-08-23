using ExamSystem.Domain.Entities.Shared;
using ExamSystem.Domain.Entities.Shared.Abstracts;
using ExamSystem.Domain.Misc;

namespace ExamSystem.Domain.Entities.McqAggregate;

public class MultipleChoiceQuestionOption : IEntity<Guid>
{
    public Guid Id { get; init; }
    public required Guid MultipleChoiceQuestionId { get; set; }
    public required ContentElement Body { get; set; }
    public required BodyType BodyType { get; set; }
    public required bool IsCorrect { get; set; }
}

public enum BodyType
{
    Text,
    Image
}
