using System.Data.Common;
using ExamSystem.Domain.Misc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ExamSystem.Infrastructure.Persistence;

public abstract class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _dbContext;

    protected UnitOfWork(DbContext examSystemDbContext) => _dbContext = examSystemDbContext;

    public virtual void Dispose() => _dbContext.Dispose();

    public virtual ValueTask DisposeAsync() => _dbContext.DisposeAsync();

    public virtual void Save() => _dbContext.SaveChanges();

    public virtual async Task SaveAsync() => await _dbContext.SaveChangesAsync();

    public async Task<DbTransaction> BeginTransactionAsync()
    {
        var trx = await _dbContext.Database.BeginTransactionAsync().ConfigureAwait(false);
        return trx.GetDbTransaction();
    }
}
