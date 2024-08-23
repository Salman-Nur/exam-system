using System.Linq.Expressions;
using ExamSystem.Domain.Entities.Shared;
using ExamSystem.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace ExamSystem.Infrastructure.Persistence.Repositories;

// Added from Devskill.Data
public interface IRepository<TEntity, in TKey>
    : IRepositoryBase<TEntity, TKey>
    where TKey : IEquatable<TKey>, IComparable
    where TEntity : class, IEntity<TKey>
{
    void Add(TEntity entity);
    Task AddAsync(TEntity entity);
    void Edit(TEntity entityToUpdate);
    Task EditAsync(TEntity entityToUpdate);

    IList<TEntity> Get(Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool isTrackingOff = false);

    (IList<TEntity> data, int total, int totalDisplay) Get(Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, int pageIndex = 1,
        int pageSize = 10, bool isTrackingOff = false);

    IList<TEntity> Get(Expression<Func<TEntity, bool>> filter,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null);

    IList<TEntity> GetAll();

    Task<IList<TEntity>> GetAsync(Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool isTrackingOff = false);

    Task<(IList<TEntity> data, int total, int totalDisplay)> GetAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, int pageIndex = 1,
        int pageSize = 10, bool isTrackingOff = false);

    Task<IList<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null);

    TEntity? GetById(TKey id);
    Task<TEntity?> GetByIdAsync(TKey id);
    int GetCount(Expression<Func<TEntity, bool>>? filter = null);

    IList<TEntity> GetDynamic(Expression<Func<TEntity, bool>>? filter = null, string? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool isTrackingOff = false);

    (IList<TEntity> data, int total, int totalDisplay) GetDynamic(
        Expression<Func<TEntity, bool>>? filter = null, string? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, int pageIndex = 1,
        int pageSize = 10, bool isTrackingOff = false);

    Task<IList<TEntity>> GetDynamicAsync(Expression<Func<TEntity, bool>>? filter = null,
        string? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool isTrackingOff = false);

    Task<(IList<TEntity> data, int total, int totalDisplay)> GetDynamicAsync(
        Expression<Func<TEntity, bool>>? filter = null, string? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, int pageIndex = 1,
        int pageSize = 10, bool isTrackingOff = false);

    void Remove(Expression<Func<TEntity, bool>> filter);
    void Remove(TEntity entityToDelete);
    void Remove(TKey id);
    Task RemoveAsync(Expression<Func<TEntity, bool>> filter);
}
