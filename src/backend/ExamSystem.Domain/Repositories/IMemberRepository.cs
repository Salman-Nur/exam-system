using ExamSystem.Domain.Entities.UnaryAggregateRoots;

namespace ExamSystem.Domain.Repositories;

public interface IMemberRepository : IRepositoryBase<Member, Guid>
{
    Task<Member?> GetMemberUserByEmailAsync(string email);
    Task UpdateMemberInformationAsync(Member memberInformation);
}
