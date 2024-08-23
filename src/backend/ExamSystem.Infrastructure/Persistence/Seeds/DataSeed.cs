using Microsoft.EntityFrameworkCore;

namespace ExamSystem.Infrastructure.Persistence.Seeds;

public abstract class DataSeed(DbContext context) : ISeed
{
    public async Task MigrateAsync()
    {
        await context.Database.MigrateAsync();
    }

    public abstract Task SeedAsync();
}
