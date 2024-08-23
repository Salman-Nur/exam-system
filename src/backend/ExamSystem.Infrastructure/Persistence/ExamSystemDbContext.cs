using ExamSystem.Domain.Entities.McqAggregate;
using ExamSystem.Domain.Entities.Shared.Abstracts;
using ExamSystem.Domain.Entities.UnaryAggregateRoots;
using ExamSystem.Domain.Misc;
using ExamSystem.Infrastructure.Persistence.Seeds;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.Infrastructure.Persistence;

public class ExamSystemDbContext : DbContext
{
    public ExamSystemDbContext(DbContextOptions<ExamSystemDbContext> options)
        : base(options)
    {
    }

    public DbSet<Log> Logs => Set<Log>();
    public DbSet<Member> Members => Set<Member>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<SystemClaim> SystemClaims => Set<SystemClaim>();
    public DbSet<MultipleChoiceQuestion> MultipleChoiceQuestions => Set<MultipleChoiceQuestion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ExamSystemDbContext).Assembly);
        modelBuilder.Entity<ImageElement>().ToTable("ImageElements");
        modelBuilder.Entity<TextElement>().ToTable("TextElements");
        modelBuilder.Entity<ContentElement>().UseTpcMappingStrategy();

        #region Data Seed

        modelBuilder.Entity<SystemClaim>()
            .HasData(SystemClaimsSeed.SystemClaims);

        #endregion
    }
}
