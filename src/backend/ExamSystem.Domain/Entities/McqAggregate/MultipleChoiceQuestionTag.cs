namespace ExamSystem.Domain.Entities.McqAggregate;

public class MultipleChoiceQuestionTag(Guid tagId, Guid multipleChoiceQuestionId)
{
    public Guid TagId { get; } = tagId;
    public Guid MultipleChoiceQuestionId { get; } = multipleChoiceQuestionId;
}
