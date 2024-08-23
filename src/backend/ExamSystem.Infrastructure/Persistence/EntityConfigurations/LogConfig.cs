using ExamSystem.Domain.Entities.Shared;
using ExamSystem.Domain.Entities.UnaryAggregateRoots;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExamSystem.Infrastructure.Persistence.EntityConfigurations;

public class LogConfig : IEntityTypeConfiguration<Log>
{
    public void Configure(EntityTypeBuilder<Log> builder)
    {
        builder.ToTable(DomainEntityConstants.LogEntityDbTableName);

        builder.Property(e => e.Id).ValueGeneratedOnAdd();

        builder.Property(e => e.Level).HasMaxLength(DomainEntityConstants.LogLevelMaxLength);
    }
}
