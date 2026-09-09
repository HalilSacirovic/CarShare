using CarShare.Application.Common.Interfaces;
using CarShare.Infrastructure.Persistence;

namespace CarShare.Infrastructure.Repositories;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly CarShareDbContext _dbContext;

    public UnitOfWork(CarShareDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}