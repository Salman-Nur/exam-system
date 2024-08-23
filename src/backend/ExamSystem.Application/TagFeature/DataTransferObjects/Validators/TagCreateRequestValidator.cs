using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExamSystem.Application.MembershipFeatures.DataTransferObjects;
using FluentValidation;

namespace ExamSystem.Application.TagFeature.DataTransferObjects.Validators
{
    public class TagCreateRequestValidator : AbstractValidator<TagCreateDTO>
    {
        public TagCreateRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotNull()
                .NotEmpty().WithMessage("Tag Name is required.");
        }
    }
}
