namespace ExamSystem.Infrastructure.Persistence.Seeds
{
    public interface ISeed
    {
        Task MigrateAsync();
        Task SeedAsync();
    }
}
