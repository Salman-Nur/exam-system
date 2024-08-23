using ExamSystem.Domain.Entities.UnaryAggregateRoots;
using ExamSystem.Domain.Repositories;

namespace ExamSystem.Infrastructure.Persistence.Repositories;

public class MemberRepository : Repository<Member, Guid>, IMemberRepository
{
    public MemberRepository(ExamSystemDbContext context) : base(context)
    {
    }
    public async Task<Member?> GetMemberUserByEmailAsync(string email)
    {
        return await GetOneAsync(filter: x => x.Email == email);
    }

    public async Task UpdateMemberInformationAsync(Member memberInformation)
    {
        await UpdateAsync(memberInformation);
    }
}
