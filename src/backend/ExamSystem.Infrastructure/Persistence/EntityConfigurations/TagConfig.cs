using ExamSystem.Domain.Entities.McqAggregate;
using ExamSystem.Domain.Entities.UnaryAggregateRoots;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExamSystem.Infrastructure.Persistence.EntityConfigurations;

public class TagConfig : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(30);

        builder.HasMany<MultipleChoiceQuestionTag>()
            .WithOne()
            .HasForeignKey(x => x.TagId)
            .IsRequired()
            .OnDelete(deleteBehavior: DeleteBehavior.Cascade);
    }
}
