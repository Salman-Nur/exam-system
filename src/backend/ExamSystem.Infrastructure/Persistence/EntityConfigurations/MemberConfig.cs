using ExamSystem.Domain.Entities.Shared;
using ExamSystem.Domain.Entities.UnaryAggregateRoots;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExamSystem.Infrastructure.Persistence.EntityConfigurations;

public class MemberConfig : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable(DomainEntityConstants.MemberEntityDbTableName);

        builder.Property(x => x.FullName)
            .HasMaxLength(DomainEntityConstants.MemberFullNameMaxLength);

        builder.Property(x => x.Email)
            .HasMaxLength(DomainEntityConstants.MemberEmailMaxLength);

        builder.Property(x => x.ProfilePictureUri)
            .HasMaxLength(DomainEntityConstants.MemberProfilePictureUriMaxLength);

        builder.HasIndex(x => x.Email).IsUnique();
    }
}
