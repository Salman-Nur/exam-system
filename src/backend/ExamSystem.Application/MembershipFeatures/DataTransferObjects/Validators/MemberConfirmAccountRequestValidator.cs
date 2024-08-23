using ExamSystem.Domain.Entities.Shared;
using FluentValidation;

namespace ExamSystem.Application.MembershipFeatures.DataTransferObjects.Validators;

public class MemberConfirmAccountRequestValidator : AbstractValidator<MemberConfirmAccountRequest>
{
    public MemberConfirmAccountRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .Length(6, DomainEntityConstants.MemberEmailMaxLength);

        RuleFor(x => x.Code).NotEmpty();

        RuleFor(x => x.RecaptchaV3ResponseCode).NotEmpty();
    }
}
