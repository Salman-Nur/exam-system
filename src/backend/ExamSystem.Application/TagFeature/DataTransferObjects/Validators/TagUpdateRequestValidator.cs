using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace ExamSystem.Application.TagFeature.DataTransferObjects.Validators
{
    public class TagUpdateRequestValidator: AbstractValidator<TagUpdateDTO>
    {
        public TagUpdateRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotNull()
                .NotEmpty().WithMessage("Tag Name is required.");
        }
    }
}
