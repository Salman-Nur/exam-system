using FluentValidation;

namespace ExamSystem.Application.MembershipFeatures.DataTransferObjects.Validators
{
    public class MemberProfileUpdateRequestValidator : AbstractValidator<MemberUpdateDTO>
    {
        public MemberProfileUpdateRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotNull()
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email address is required.");

            RuleFor(x => x.FullName)
                .NotNull()
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(100).WithMessage("Full name must not exceed 100 characters.");
        }
    }
}
