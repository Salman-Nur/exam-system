using ExamSystem.Application.Common.Contracts;
using ExamSystem.Application.MembershipFeatures.DataTransferObjects;
using ExamSystem.Domain.Entities.UnaryAggregateRoots;
using ExamSystem.Domain.Paging;
using SharpOutcome;
using SharpOutcome.Helpers;
using Mapster;
using ExamSystem.Application.MembershipFeatures.Enums;

namespace ExamSystem.Application.MembershipFeatures.Services;

public class MembershipService : IMembershipService
{
    private readonly IMembershipEmailSenderService _membershipEmailSenderService;
    private readonly IApplicationUnitOfWork _appUnitOfWork;

    public MembershipService(IApplicationUnitOfWork appUnitOfWork,
        IMembershipEmailSenderService membershipEmailSenderService)
    {
        _appUnitOfWork = appUnitOfWork;
        _membershipEmailSenderService = membershipEmailSenderService;
    }

    public async Task<Member?> GetOneAsync(Guid id, CancellationToken ct)
    {
        return await _appUnitOfWork.MemberRepository.GetOneAsync(
            filter: x => x.Id == id,
            cancellationToken: ct
        );
    }

    public async Task SendVerificationMailAsync(string fullName, string email, string verificationCode)
    {
        await _membershipEmailSenderService.SendVerificationMailAsync(fullName, email, verificationCode);
    }

    public async Task SendResetPasswordMailAsync(string fullName, string email, string code)
    {
        await _membershipEmailSenderService.SendResetPasswordMailAsync(fullName, email, code);
    }

    public async Task<IPaginate<MemberListDTO>> GetMembersAsync(SearchRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var result = await _appUnitOfWork.MemberRepository.GetPagedListAsync(
            selector: x => new Member
            {
                Id = x.Id,
                FullName = x.FullName,
                Email = x.Email,
                ProfilePictureUri = x.ProfilePictureUri,
                Status = x.Status,
                CreatedAtUtc = x.CreatedAtUtc
            }, request);

        if (result is { Items.Count: > 0 })
        {
            return new Paginate<MemberListDTO>([], request.PageIndex, request.PageSize, 0);
        }

        var mappedResult = result.Items.Adapt<List<MemberListDTO>>();

        return new Paginate<MemberListDTO>(mappedResult, request.PageIndex, request.PageSize, result.Total);
    }

    public async Task<MemberUpdateDTO> GetMemberUserByEmailAsync(string email)
    {
        var memberInfo = await _appUnitOfWork.MemberRepository.GetMemberUserByEmailAsync(email);
        var memberUpdateDto = await memberInfo.BuildAdapter().AdaptToTypeAsync<MemberUpdateDTO>();
        return memberUpdateDto;
    }

    public async Task<ValueOutcome<Successful, MembershipErrorReason>> UpdateMemberInformationAsync(
        MemberUpdateDTO memberUpdateDto, string email)
    {
        var memberInfo = await _appUnitOfWork.MemberRepository.GetMemberUserByEmailAsync(email);
        if (memberInfo is null)
        {
            return MembershipErrorReason.NotFound;
        }

        memberInfo = await memberUpdateDto.BuildAdapter().AdaptToAsync(memberInfo);
        await _appUnitOfWork.MemberRepository.UpdateMemberInformationAsync(memberInfo);
        await _appUnitOfWork.SaveAsync();
        return new Successful();
    }
}
