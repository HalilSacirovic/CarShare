using CarShare.Application.Common.Interfaces;
using CarShare.Domain.Common;
using CarShare.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CarShare.Infrastructure.Repositories;

public sealed class Repository<T> : IRepository<T>
    where T : BaseEntity
{
    private readonly CarShareDbContext _dbContext;
    private readonly DbSet<T> _dbSet;

    public Repository(CarShareDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = dbContext.Set<T>();
    }

    public async Task<T?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(
            new object[] { id },
            cancellationToken);
    }

    public async Task<List<T>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public IQueryable<T> Query()
    {
        return _dbSet.AsQueryable();
    }

    public async Task AddAsync(
        T entity,
        CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public void Update(T entity)
    {
        entity.UpdatedAtUtc = DateTime.UtcNow;
        _dbSet.Update(entity);
    }

    public void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }
}