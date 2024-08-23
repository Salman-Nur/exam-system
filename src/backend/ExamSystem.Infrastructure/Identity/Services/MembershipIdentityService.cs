using ExamSystem.Application.Common.Contracts;
using ExamSystem.Application.Common.Providers;
using ExamSystem.Application.MembershipFeatures.DataTransferObjects;
using ExamSystem.Application.MembershipFeatures.Enums;
using ExamSystem.Application.MembershipFeatures.Results;
using ExamSystem.Domain.Entities.UnaryAggregateRoots;
using ExamSystem.Domain.Enums;
using ExamSystem.Infrastructure.Identity.Managers;
using Microsoft.Extensions.Logging;
using SharpOutcome;

namespace ExamSystem.Infrastructure.Identity.Services;

public class MembershipIdentityService : IMembershipIdentityService
{
    private readonly ILogger<MembershipIdentityService> _logger;
    private readonly IApplicationUnitOfWork _appUnitOfWork;
    private readonly ApplicationUserManager _appUserManager;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IGuidProvider _guidProvider;

    public MembershipIdentityService(IApplicationUnitOfWork appUnitOfWork,
        ApplicationUserManager appUserManager, IDateTimeProvider dateTimeProvider,
        IGuidProvider guidProvider, ILogger<MembershipIdentityService> logger)
    {
        _appUnitOfWork = appUnitOfWork;
        _appUserManager = appUserManager;
        _dateTimeProvider = dateTimeProvider;
        _guidProvider = guidProvider;
        _logger = logger;
    }

    public async Task<Outcome<(ApplicationIdentityUser appIdentityUser, Member member), MembershipError>>
        SignupAsync(MemberSignupRequest dto)
    {
        await using var trx = await _appUnitOfWork.BeginTransactionAsync();
        try
        {
            var applicationIdentityUser = new ApplicationIdentityUser
            {
                Id = _guidProvider.SortableGuid(),
                Email = dto.Email,
                UserName = dto.Email
            };

            var isMemberExists = await _appUnitOfWork.MemberRepository.GetOneAsync(
                filter: x => x.Email == dto.Email,
                disableTracking: true
            );

            if (isMemberExists is not null)
            {
                await trx.RollbackAsync();
                return new MembershipError(MembershipErrorReason.DuplicateEmail, []);
            }

            var result = await _appUserManager.CreateAsync(applicationIdentityUser, dto.Password);
            if (result.Succeeded is false)
            {
                List<KeyValuePair<string, string>> errors = [];
                var isDuplicateEmail =
                    result.Errors.FirstOrDefault(x => x.Code == "DuplicateEmail");

                if (isDuplicateEmail is not null)
                {
                    errors.Add(new KeyValuePair<string, string>(isDuplicateEmail.Code,
                        isDuplicateEmail.Description));
                    await trx.RollbackAsync();
                    return new MembershipError(MembershipErrorReason.DuplicateEmail, errors);
                }

                errors.AddRange(result.Errors.Select(err =>
                    new KeyValuePair<string, string>(err.Code, err.Description)));

                await trx.RollbackAsync();
                return new MembershipError(MembershipErrorReason.Others, errors);
            }

            var member = new Member
            {
                Id = applicationIdentityUser.Id,
                FullName = dto.FullName,
                Email = dto.Email,
                Status = UserStatus.Active,
                CreatedAtUtc = _dateTimeProvider.CurrentUtcTime
            };

            await _appUnitOfWork.MemberRepository.CreateAsync(member);
            await _appUnitOfWork.SaveAsync();

            await trx.CommitAsync();
            return (applicationIdentityUser, member);
        }
        catch (Exception exception)
        {
            await trx.RollbackAsync();
            _logger.LogError("{exception}", exception);
            return new MembershipError(MembershipErrorReason.Unknown, []);
        }
    }

    public Task<string> IssueVerificationTokenAsync(ApplicationIdentityUser user)
    {
        return _appUserManager.GenerateEmailConfirmationTokenAsync(user);
    }

    public async Task<Outcome<(ApplicationIdentityUser appIdentityUser, string token),
        ForgotPasswordErrorReason>> RequestPasswordResetAsync(string email)
    {
        var appIdentityUser = await _appUserManager.FindByEmailAsync(email);
        if (appIdentityUser is null)
        {
            return ForgotPasswordErrorReason.UserNotFound;
        }

        if (await _appUserManager.IsEmailConfirmedAsync(appIdentityUser) is false)
        {
            return ForgotPasswordErrorReason.ProfileNotConfirmed;
        }

        var token = await _appUserManager.GeneratePasswordResetTokenAsync(appIdentityUser);
        return (appIdentityUser, token);
    }

    public async Task<PasswordResetResult> ResetPasswordAsync(MemberResetPasswordRequest dto)
    {
        var appIdentityUser = await _appUserManager.FindByEmailAsync(dto.Email);
        if (appIdentityUser is null)
        {
            return PasswordResetResult.UserNotFound;
        }

        if (await _appUserManager.IsEmailConfirmedAsync(appIdentityUser) is false)
        {
            return PasswordResetResult.ProfileNotConfirmed;
        }

        var isSameAsOld = await _appUserManager.CheckPasswordAsync(appIdentityUser, dto.Password);

        if (isSameAsOld)
        {
            return PasswordResetResult.SameAsOldPassword;
        }

        var result = await _appUserManager.ResetPasswordAsync(appIdentityUser, dto.Code, dto.Password);

        return result.Succeeded is false ? PasswordResetResult.InvalidToken : PasswordResetResult.Ok;
    }

    public async Task<Outcome<ApplicationIdentityUser, CredentialErrorReason>>
        CanRequestEmailConfirmationAsync(MemberResendVerificationCodeRequest dto)
    {
        var user = await _appUserManager.FindByEmailAsync(dto.Email);
        if (user?.PasswordHash is null)
        {
            return CredentialErrorReason.UserNotFound;
        }

        if (await _appUserManager.CheckPasswordAsync(user, dto.Password) is false)
        {
            return CredentialErrorReason.PasswordNotMatched;
        }

        if (user.EmailConfirmed)
        {
            return CredentialErrorReason.ProfileAlreadyConfirmed;
        }

        return user;
    }
}
