using ExamSystem.Domain.Entities.McqAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExamSystem.Infrastructure.Persistence.EntityConfigurations;

public class McqTagConfig : IEntityTypeConfiguration<MultipleChoiceQuestionTag>
{
    public void Configure(EntityTypeBuilder<MultipleChoiceQuestionTag> builder)
    {
        builder.HasKey(x => new {x.TagId, x.MultipleChoiceQuestionId});
    }
}
