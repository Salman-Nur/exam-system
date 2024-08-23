using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using ExamSystem.Domain.Entities.Shared;
using ExamSystem.Domain.Paging;
using ExamSystem.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace ExamSystem.Infrastructure.Persistence.Repositories;

public abstract class Repository<TEntity, TKey>
    : IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : IEquatable<TKey>, IComparable
{
    private readonly DbContext _dbContext;
    private readonly DbSet<TEntity> _entityDbSet;

    protected Repository(DbContext context)
    {
        _dbContext = context;
        _entityDbSet = _dbContext.Set<TEntity>();
    }

    public virtual async Task CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _entityDbSet.AddAsync(entity, cancellationToken).ConfigureAwait(false);
    }

    public async Task CreateManyAsync(ICollection<TEntity> entity)
    {
        await _entityDbSet.AddRangeAsync(entity).ConfigureAwait(false);
    }

    public virtual async Task<TEntity?> GetOneAsync(
        Expression<Func<TEntity, bool>> filter,
        bool useSplitQuery = false,
        ICollection<Expression<Func<TEntity, object?>>>? includes = null,
        bool disableTracking = false,
        CancellationToken cancellationToken = default
    )
    {
        var data = _entityDbSet.Where(filter);
        if (includes is not null)
        {
            data = includes.Aggregate(data, (current, include) => current.Include(include));
        }

        if (useSplitQuery)
        {
            data = data.AsSplitQuery();
        }

        if (disableTracking)
        {
            data = data.AsNoTracking();
        }

        return await data.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task<TEntity?> GetOneAsync<TSorter>(
        Expression<Func<TEntity, bool>> filter,
        Expression<Func<TEntity, TSorter>> orderBy,
        bool useSplitQuery = false,
        bool ascendingOrder = true,
        IList<Expression<Func<TEntity, object>>>? includes = null,
        bool disableTracking = false,
        CancellationToken cancellationToken = default
    ) where TSorter : IComparable<TSorter>
    {
        var data = _entityDbSet.Where(filter);

        data = ascendingOrder
            ? data.OrderBy(orderBy)
            : data.OrderByDescending(orderBy);

        if (includes is not null)
        {
            data = includes.Aggregate(data, (current, include) => current.Include(include));
        }

        if (useSplitQuery)
        {
            data = data.AsSplitQuery();
        }

        if (disableTracking)
        {
            data = data.AsNoTracking();
        }

        return await data.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task<TResult?> GetOneAsync<TResult>(
        Expression<Func<TEntity, bool>> filter,
        Expression<Func<TEntity, TResult>> subsetSelector,
        bool useSplitQuery = false,
        IList<Expression<Func<TEntity, object>>>? includes = null,
        bool disableTracking = false,
        CancellationToken cancellationToken = default
    )
    {
        var data = _entityDbSet.Where(filter);
        if (includes is not null && includes.Count > 0)
        {
            data = includes.Aggregate(data, (current, include) => current.Include(include));
        }

        if (useSplitQuery)
        {
            data = data.AsSplitQuery();
        }

        if (disableTracking)
        {
            data = data.AsNoTracking();
        }

        return await data.Select(subsetSelector)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public virtual async Task<TResult?> GetOneAsync<TResult, TSorter>(
        Expression<Func<TEntity, bool>> filter,
        Expression<Func<TEntity, TResult>> subsetSelector,
        Expression<Func<TEntity, TSorter>> orderBy,
        bool useSplitQuery = false,
        bool ascendingOrder = true,
        IList<Expression<Func<TEntity, object>>>? includes = null,
        bool disableTracking = false,
        CancellationToken cancellationToken = default
    ) where TSorter : IComparable<TSorter>
    {
        var data = _entityDbSet.Where(filter);

        if (useSplitQuery)
        {
            data = data.AsSplitQuery();
        }

        if (disableTracking)
        {
            data = data.AsNoTracking();
        }

        data = ascendingOrder
            ? data.OrderBy(orderBy)
            : data.OrderByDescending(orderBy);

        if (includes is not null && includes.Count > 0)
        {
            data = includes.Aggregate(data, (current, include) => current.Include(include));
        }

        return await data.Select(subsetSelector)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public virtual async Task<IList<TEntity>> GetAllAsync(
        bool useSplitQuery = false,
        int page = 1, int limit = 10,
        ICollection<Expression<Func<TEntity, object>>>? includes = null,
        bool disableTracking = false,
        CancellationToken cancellationToken = default
    )
    {
        var data = _entityDbSet.AsQueryable();
        if (includes is not null)
        {
            data = includes.Aggregate(data, (current, include) => current.Include(include));
        }

        var safePagination = AvoidNegativeOrZeroPagination(page, limit);
        data = data.Skip((safePagination.page - 1) * safePagination.limit).Take(safePagination.limit);

        if (useSplitQuery)
        {
            data = data.AsSplitQuery();
        }

        if (disableTracking)
        {
            data = data.AsNoTracking();
        }

        return await data.ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task<IList<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>> filter,
        bool useSplitQuery = false,
        int page = 1, int limit = 10,
        ICollection<Expression<Func<TEntity, object>>>? includes = null,
        bool disableTracking = false,
        CancellationToken cancellationToken = default
    )
    {
        var data = _entityDbSet.Where(filter);

        if (includes is not null)
        {
            data = includes.Aggregate(data, (current, include) => current.Include(include));
        }

        var safePagination = AvoidNegativeOrZeroPagination(page, limit);
        data = data.Skip((safePagination.page - 1) * safePagination.limit).Take(safePagination.limit);

        if (useSplitQuery)
        {
            data = data.AsSplitQuery();
        }

        if (disableTracking)
        {
            data = data.AsNoTracking();
        }

        return await data.ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task<IList<TEntity>> GetAllAsync<TSorter>(
        Expression<Func<TEntity, TSorter>> orderBy,
        bool useSplitQuery = false,
        int page = 1,
        int limit = 10,
        bool ascendingOrder = true,
        ICollection<Expression<Func<TEntity, object>>>? includes = null,
        bool disableTracking = false,
        CancellationToken cancellationToken = default
    )
    {
        var data = _entityDbSet.AsQueryable();
        if (includes is not null)
        {
            data = includes.Aggregate(data, (current, include) => current.Include(include));
        }

        data = ascendingOrder
            ? data.OrderBy(orderBy)
            : data.OrderByDescending(orderBy);

        var safePagination = AvoidNegativeOrZeroPagination(page, limit);
        data = data.Skip((safePagination.page - 1) * safePagination.limit).Take(safePagination.limit);

        if (useSplitQuery)
        {
            data = data.AsSplitQuery();
        }

        if (disableTracking)
        {
            data = data.AsNoTracking();
        }

        return await data.ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<IList<TEntity>> GetAllAsync<TSorter>(
        Expression<Func<TEntity, bool>> filter,
        Expression<Func<TEntity, TSorter>> orderBy,
        bool useSplitQuery = false,
        int page = 1, int limit = 10,
        bool ascendingOrder = true,
        ICollection<Expression<Func<TEntity, object>>>? includes = null,
        bool disableTracking = false,
        CancellationToken cancellationToken = default)
    {
        var data = _entityDbSet.Where(filter);

        if (includes is not null)
        {
            data = includes.Aggregate(data, (current, include) => current.Include(include));
        }

        data = ascendingOrder
            ? data.OrderBy(orderBy)
            : data.OrderByDescending(orderBy);

        var safePagination = AvoidNegativeOrZeroPagination(page, limit);
        data = data.Skip((safePagination.page - 1) * safePagination.limit).Take(safePagination.limit);

        if (useSplitQuery)
        {
            data = data.AsSplitQuery();
        }

        if (disableTracking)
        {
            data = data.AsNoTracking();
        }

        return await data.ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task<IList<TResult>> GetAllAsync<TResult, TSorter>(
        Expression<Func<TEntity, TResult>> subsetSelector,
        Expression<Func<TEntity, TSorter>> orderBy,
        bool useSplitQuery = false,
        int page = 1,
        int limit = 10,
        bool ascendingOrder = true,
        ICollection<Expression<Func<TEntity, object>>>? includes = null,
        bool disableTracking = false,
        CancellationToken cancellationToken = default
    )
    {
        var data = _entityDbSet.AsQueryable();
        if (includes is not null)
        {
            data = includes.Aggregate(data, (current, include) => current.Include(include));
        }

        data = ascendingOrder
            ? data.OrderBy(orderBy)
            : data.OrderByDescending(orderBy);

        var safePagination = AvoidNegativeOrZeroPagination(page, limit);
        data = data.Skip((safePagination.page - 1) * safePagination.limit).Take(safePagination.limit);

        if (useSplitQuery)
        {
            data = data.AsSplitQuery();
        }

        if (disableTracking)
        {
            data = data.AsNoTracking();
        }

        return await data.Select(subsetSelector)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IList<TResult>> GetAllAsync<TResult, TSorter>(
        Expression<Func<TEntity, bool>> filter,
        Expression<Func<TEntity, TResult>> subsetSelector,
        Expression<Func<TEntity, TSorter>> orderBy,
        bool useSplitQuery = false,
        int page = 1,
        int limit = 10, bool ascendingOrder = true,
        ICollection<Expression<Func<TEntity, object>>>? includes = null,
        bool disableTracking = false,
        CancellationToken cancellationToken = default)
    {
        var data = _entityDbSet.Where(filter);
        if (includes is not null)
        {
            data = includes.Aggregate(data, (current, include) => current.Include(include));
        }

        data = ascendingOrder
            ? data.OrderBy(orderBy)
            : data.OrderByDescending(orderBy);

        var safePagination = AvoidNegativeOrZeroPagination(page, limit);
        data = data.Skip((safePagination.page - 1) * safePagination.limit).Take(safePagination.limit);

        if (useSplitQuery)
        {
            data = data.AsSplitQuery();
        }

        if (disableTracking)
        {
            data = data.AsNoTracking();
        }

        return await data.Select(subsetSelector)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public virtual Task UpdateAsync(TEntity entityToUpdate)
    {
        return Task.Run(() =>
        {
            _entityDbSet.Attach(entityToUpdate);
            _dbContext.Entry(entityToUpdate).State = EntityState.Modified;
        });
    }

    public virtual Task RemoveAsync(TEntity entityToDelete)
    {
        return Task.Run(() =>
        {
            if (_dbContext.Entry(entityToDelete).State is EntityState.Detached)
            {
                _entityDbSet.Attach(entityToDelete);
            }

            _entityDbSet.Remove(entityToDelete);
        });
    }

    public virtual async Task<int> GetCountAsync(Expression<Func<TEntity, bool>>? filter = null,
        CancellationToken cancellationToken = default)
    {
        int count;

        if (filter is not null)
        {
            count = await _entityDbSet
                .CountAsync(filter, cancellationToken)
                .ConfigureAwait(false);
        }

        else
        {
            count = await _entityDbSet.CountAsync(cancellationToken).ConfigureAwait(false);
        }

        return count;
    }

    public Task TrackEntityAsync<T>(T entity) where T : class
    {
        return Task.Run(() => _dbContext.Set<T>().Attach(entity));
    }

    public Task TrackEntityAsync(TEntity entity)
    {
        return Task.Run(() => _dbContext.Set<TEntity>().Attach(entity));
    }

    public Task ModifyEntityStateToAddedAsync(TEntity entity)
    {
        return Task.Run(() =>
        {
            if (_dbContext.Entry(entity).State is not EntityState.Added)
            {
                _dbContext.Entry(entity).State = EntityState.Added;
            }
        });
    }

    public Task ModifyEntityStateToAddedAsync<T>(T entity)
    {
        return Task.Run(() =>
        {
            if (entity is null)
            {
                return;
            }

            if (_dbContext.Entry(entity).State is not EntityState.Added)
            {
                _dbContext.Entry(entity).State = EntityState.Added;
            }
        });
    }

    private static (int page, int limit) AvoidNegativeOrZeroPagination(int page, int limit)
    {
        var pagination = (page, limit);
        if (page <= 0)
        {
            pagination.page = 1;
        }

        if (limit <= 0)
        {
            pagination.limit = 1;
        }

        return pagination;
    }

    // Added form Devskill.Data

    public virtual async Task AddAsync(TEntity entity)
    {
        await _entityDbSet.AddAsync(entity);
    }

    public virtual Task RemoveAsync(Expression<Func<TEntity, bool>> filter)
    {
        return Task.Run(() => { _entityDbSet.RemoveRange(_entityDbSet.Where(filter)); });
    }

    public virtual Task EditAsync(TEntity entityToUpdate) =>
        Task.Run(() =>
        {
            _entityDbSet.Attach(entityToUpdate);
            _dbContext.Entry(entityToUpdate).State = EntityState.Modified;
        });

    public virtual async Task<TEntity?> GetByIdAsync(TKey id)
    {
        return await _entityDbSet.FindAsync(id);
    }

    public virtual int GetCount(Expression<Func<TEntity, bool>>? filter = null)
    {
        IQueryable<TEntity> query = _entityDbSet;
        return filter is not null
            ? query.Count(filter)
            : query.Count();
    }

    public virtual async Task<IList<TEntity>> GetAsync(Expression<Func<TEntity, bool>>? filter,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
    {
        IQueryable<TEntity> query = _entityDbSet;

        if (filter is not null)
        {
            query = query.Where(filter);
        }

        if (include is not null)
        {
            query = include(query);
        }

        return await query.ToListAsync();
    }

    public virtual async Task<(IList<TEntity> data, int total, int totalDisplay)> GetAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int pageIndex = 1,
        int pageSize = 10,
        bool isTrackingOff = false)
    {
        IQueryable<TEntity> query = _entityDbSet;
        var total = query.Count();
        var totalDisplay = query.Count();

        if (filter is not null)
        {
            query = query.Where(filter);
            totalDisplay = query.Count();
        }

        if (include is not null)
        {
            query = include(query);
        }

        IList<TEntity> data;

        if (orderBy is not null)
        {
            var result = orderBy(query).Skip((pageIndex - 1) * pageSize).Take(pageSize);

            if (isTrackingOff)
            {
                data = await result.AsNoTracking().ToListAsync();
            }
            else
            {
                data = await result.ToListAsync();
            }
        }
        else
        {
            var result = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            if (isTrackingOff)
            {
                data = await result.AsNoTracking().ToListAsync();
            }
            else
            {
                data = await result.ToListAsync();
            }
        }

        return (data, total, totalDisplay);
    }

    public virtual async Task<(IList<TEntity> data, int total, int totalDisplay)> GetDynamicAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        string? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int pageIndex = 1,
        int pageSize = 10,
        bool isTrackingOff = false)
    {
        IQueryable<TEntity> query = _entityDbSet;
        var total = query.Count();
        var totalDisplay = query.Count();

        if (filter is not null)
        {
            query = query.Where(filter);
            totalDisplay = query.Count();
        }

        if (include is not null)
        {
            query = include(query);
        }

        IList<TEntity> data;

        if (orderBy is not null)
        {
            var result = query.OrderBy(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize);

            if (isTrackingOff)
            {
                data = await result.AsNoTracking().ToListAsync();
            }
            else
            {
                data = await result.ToListAsync();
            }
        }
        else
        {
            var result = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            if (isTrackingOff)
            {
                data = await result.AsNoTracking().ToListAsync();
            }
            else
            {
                data = await result.ToListAsync();
            }
        }

        return (data, total, totalDisplay);
    }

    public virtual async Task<IList<TEntity>> GetAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool isTrackingOff = false)
    {
        IQueryable<TEntity> query = _entityDbSet;

        if (filter is not null)
        {
            query = query.Where(filter);
        }

        if (include is not null)
        {
            query = include(query);
        }

        if (orderBy is null)
        {
            return isTrackingOff
                ? await query.AsNoTracking().ToListAsync()
                : await query.ToListAsync();
        }

        var result = orderBy(query);
        return isTrackingOff
            ? await result.AsNoTracking().ToListAsync()
            : await result.ToListAsync();
    }

    public virtual async Task<IList<TEntity>> GetDynamicAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        string? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool isTrackingOff = false)
    {
        IQueryable<TEntity> query = _entityDbSet;

        if (filter is not null)
        {
            query = query.Where(filter);
        }

        if (include is not null)
        {
            query = include(query);
        }

        if (orderBy is null)
        {
            return isTrackingOff
                ? await query.AsNoTracking().ToListAsync()
                : await query.ToListAsync();
        }

        var result = query.OrderBy(orderBy);
        return isTrackingOff
            ? await result.AsNoTracking().ToListAsync()
            : await result.ToListAsync();
    }

    public virtual void Add(TEntity entity)
    {
        _entityDbSet.Add(entity);
    }

    public virtual void Remove(TKey id)
    {
        var entityToDelete = _entityDbSet.Find(id);
        if (entityToDelete is not null)
        {
            Remove(entityToDelete);
        }
    }

    public virtual void Remove(TEntity entityToDelete)
    {
        if (_dbContext.Entry(entityToDelete).State == EntityState.Detached)
        {
            _entityDbSet.Attach(entityToDelete);
        }

        _entityDbSet.Remove(entityToDelete);
    }

    public virtual void Remove(Expression<Func<TEntity, bool>> filter)
    {
        _entityDbSet.RemoveRange(_entityDbSet.Where(filter));
    }

    public virtual void Edit(TEntity entityToUpdate)
    {
        _entityDbSet.Attach(entityToUpdate);
        _dbContext.Entry(entityToUpdate).State = EntityState.Modified;
    }

    public virtual IList<TEntity> Get(Expression<Func<TEntity, bool>>? filter,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
    {
        IQueryable<TEntity> query = _entityDbSet;

        if (filter is not null)
        {
            query = query.Where(filter);
        }

        if (include is not null)
        {
            query = include(query);
        }

        return query.ToList();
    }

    public virtual IList<TEntity> GetAll()
    {
        IQueryable<TEntity> query = _entityDbSet;
        return query.ToList();
    }

    public virtual TEntity? GetById(TKey id)
    {
        return _entityDbSet.Find(id);
    }

    public virtual (IList<TEntity> data, int total, int totalDisplay) Get(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int pageIndex = 1, int pageSize = 10, bool isTrackingOff = false)
    {
        IQueryable<TEntity> query = _entityDbSet;
        var total = query.Count();
        var totalDisplay = query.Count();

        if (filter is not null)
        {
            query = query.Where(filter);
            totalDisplay = query.Count();
        }

        if (include is not null)
        {
            query = include(query);
        }

        if (orderBy is not null)
        {
            var result = orderBy(query).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            return isTrackingOff
                ? (result.AsNoTracking().ToList(), total, totalDisplay)
                : (result.ToList(), total, totalDisplay);
        }
        else
        {
            var result = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
            return isTrackingOff
                ? (result.AsNoTracking().ToList(), total, totalDisplay)
                : (result.ToList(), total, totalDisplay);
        }
    }

    public virtual (IList<TEntity> data, int total, int totalDisplay) GetDynamic(
        Expression<Func<TEntity, bool>>? filter = null,
        string? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int pageIndex = 1, int pageSize = 10, bool isTrackingOff = false)
    {
        IQueryable<TEntity> query = _entityDbSet;
        var total = query.Count();
        var totalDisplay = query.Count();

        if (filter is not null)
        {
            query = query.Where(filter);
            totalDisplay = query.Count();
        }

        if (include is not null)
        {
            query = include(query);
        }

        if (orderBy is not null)
        {
            var result = query.OrderBy(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            return isTrackingOff
                ? (result.AsNoTracking().ToList(), total, totalDisplay)
                : (result.ToList(), total, totalDisplay);
        }
        else
        {
            var result = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
            return isTrackingOff
                ? (result.AsNoTracking().ToList(), total, totalDisplay)
                : (result.ToList(), total, totalDisplay);
        }
    }

    public virtual IList<TEntity> Get(Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool isTrackingOff = false)
    {
        IQueryable<TEntity> query = _entityDbSet;

        if (filter is not null)
        {
            query = query.Where(filter);
        }

        if (include is not null)
        {
            query = include(query);
        }

        if (orderBy is null)
        {
            return isTrackingOff
                ? query.AsNoTracking().ToList()
                : query.ToList();
        }

        var result = orderBy(query);
        return isTrackingOff
            ? result.AsNoTracking().ToList()
            : result.ToList();
    }

    public virtual IList<TEntity> GetDynamic(Expression<Func<TEntity, bool>>? filter = null,
        string? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool isTrackingOff = false)
    {
        IQueryable<TEntity> query = _entityDbSet;

        if (filter is not null)
        {
            query = query.Where(filter);
        }

        if (include is not null)
        {
            query = include(query);
        }

        if (orderBy is null)
        {
            return isTrackingOff
                ? query.AsNoTracking().ToList()
                : query.ToList();
        }

        var result = query.OrderBy(orderBy);
        return isTrackingOff
            ? result.AsNoTracking().ToList()
            : result.ToList();
    }

    public async Task<IPaginate<TResult>> GetPagedListAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        SearchRequest request, bool spiltQuery = false,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        CancellationToken cancellationToken = default) where TResult : class
    {
        var query = _entityDbSet.AsQueryable().AsNoTracking();
        var total = await query.CountAsync(cancellationToken).ConfigureAwait(false);
        if (spiltQuery)
        {
            query = query.AsSplitQuery();
        }

        if (include is not null)
        {
            query = include(query);
        }

        var querySelector = query.Select(selector);
        return await querySelector.Where(request.Filters).OrderBy(request.Sorts)
            .ToPaginateAsync(request.PageIndex, request.PageSize, total, 1, cancellationToken)
            .ConfigureAwait(false);
    }
}
