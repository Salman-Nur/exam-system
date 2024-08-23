using ExamSystem.Domain.Entities.Shared;
using FluentValidation;

namespace ExamSystem.Application.MembershipFeatures.DataTransferObjects.Validators;

public class MemberResendVerificationCodeRequestValidator
    : AbstractValidator<MemberResendVerificationCodeRequest>
{
    public MemberResendVerificationCodeRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .Length(6, DomainEntityConstants.MemberEmailMaxLength);

        RuleFor(x => x.Password)
            .NotEmpty()
            .Length(6, 128);

        RuleFor(x => x.RecaptchaV3ResponseCode).NotEmpty();
    }
}
