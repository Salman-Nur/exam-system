using ExamSystem.Domain.Misc;
using ExamSystem.Domain.Repositories;

namespace ExamSystem.Application.Common.Contracts;

public interface IApplicationUnitOfWork : IUnitOfWork
{
    public IMemberRepository MemberRepository { get; }
    public IQuestionRepository QuestionRepository { get; }
    public ITagRepository TagRepository { get; }
}
