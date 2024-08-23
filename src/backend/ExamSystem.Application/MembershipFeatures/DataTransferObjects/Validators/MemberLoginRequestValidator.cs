using ExamSystem.Domain.Entities.Shared;
using FluentValidation;

namespace ExamSystem.Application.MembershipFeatures.DataTransferObjects.Validators;

public class MemberLoginRequestValidator : AbstractValidator<MemberLoginRequest>
{
    public MemberLoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .Length(6, DomainEntityConstants.MemberEmailMaxLength);

        RuleFor(x => x.Password)
            .NotEmpty()
            .Length(6, 128);

        RuleFor(x => x.RememberMe)
            .NotNull();

        RuleFor(x => x.RecaptchaV3ResponseCode).NotEmpty();
    }
}
