using ExamSystem.Application.MembershipFeatures.DataTransferObjects;
using ExamSystem.Domain.Entities.UnaryAggregateRoots;
using ExamSystem.Domain.Paging;
using SharpOutcome;
using ExamSystem.Application.MembershipFeatures.Enums;
using SharpOutcome.Helpers;

namespace ExamSystem.Application.MembershipFeatures.Services;

public interface IMembershipService
{
    public Task SendVerificationMailAsync(string fullName, string email, string verificationCode);
    public Task SendResetPasswordMailAsync(string fullName, string email, string code);
    public Task<Member?> GetOneAsync(Guid id, CancellationToken ct);
    Task<IPaginate<MemberListDTO>> GetMembersAsync(SearchRequest request);
    Task<MemberUpdateDTO> GetMemberUserByEmailAsync(string email);
    Task<ValueOutcome<Successful, MembershipErrorReason>> UpdateMemberInformationAsync(MemberUpdateDTO memberUpdateDto, string email);
}
