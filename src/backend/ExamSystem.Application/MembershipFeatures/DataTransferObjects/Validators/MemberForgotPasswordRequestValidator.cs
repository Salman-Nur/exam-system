using ExamSystem.Domain.Entities.Shared;
using FluentValidation;

namespace ExamSystem.Application.MembershipFeatures.DataTransferObjects.Validators;

public class MemberForgotPasswordRequestValidator : AbstractValidator<MemberForgotPasswordRequest>
{
    public MemberForgotPasswordRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .Length(6, DomainEntityConstants.MemberEmailMaxLength);

        RuleFor(x => x.RecaptchaV3ResponseCode).NotEmpty();
    }
}
