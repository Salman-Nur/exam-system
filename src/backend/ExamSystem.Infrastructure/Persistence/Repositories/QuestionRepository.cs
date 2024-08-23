using ExamSystem.Domain.Entities.McqAggregate;
using ExamSystem.Domain.Repositories;

namespace ExamSystem.Infrastructure.Persistence.Repositories;

public class QuestionRepository(ExamSystemDbContext context)
    : Repository<MultipleChoiceQuestion, Guid>(context), IQuestionRepository;
