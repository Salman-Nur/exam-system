using ExamSystem.Domain.Entities.McqAggregate;

namespace ExamSystem.Domain.Repositories;

public interface IQuestionRepository : IRepositoryBase<MultipleChoiceQuestion, Guid>
{

}
