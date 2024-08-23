using ExamSystem.Domain.Entities.McqAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExamSystem.Infrastructure.Persistence.EntityConfigurations;

public class MultipleChoiceQuestionConfig : IEntityTypeConfiguration<MultipleChoiceQuestion>
{
    public void Configure(EntityTypeBuilder<MultipleChoiceQuestion> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasMany(x => x.Options)
            .WithOne()
            .HasForeignKey(x => x.MultipleChoiceQuestionId)
            .OnDelete(deleteBehavior: DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasMany(x => x.Tags)
            .WithOne()
            .HasForeignKey(x => x.MultipleChoiceQuestionId)
            .OnDelete(deleteBehavior: DeleteBehavior.Cascade)
            .IsRequired();
    }
}
