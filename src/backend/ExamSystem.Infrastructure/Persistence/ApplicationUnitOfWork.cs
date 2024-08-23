using ExamSystem.Application.Common.Contracts;
using ExamSystem.Domain.Repositories;

namespace ExamSystem.Infrastructure.Persistence;

public class ApplicationUnitOfWork : UnitOfWork, IApplicationUnitOfWork
{
    public IMemberRepository MemberRepository { get; }
	public IQuestionRepository QuestionRepository { get; }
    public ITagRepository TagRepository { get; }
    public ApplicationUnitOfWork(
        ExamSystemDbContext examSystemDbContext,
        IMemberRepository memberRepository,
		IQuestionRepository questionRepository,
        ITagRepository tagRepository)
        : base(examSystemDbContext)
    {
        MemberRepository = memberRepository;
        QuestionRepository = questionRepository;
        TagRepository = tagRepository;
    }
}
