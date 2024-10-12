using Microsoft.EntityFrameworkCore;
using TurnDigital.Domain.DataAccess.Interfaces;
using TurnDigital.Domain.Entities.Interfaces;

namespace TurnDigital.Infrastructure.DataAccess;

internal class Repository : IRepository
{
    private readonly TurnDigitalDbContext _dbContext;

    public Repository(TurnDigitalDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public DbSet<TEntity> GetEntitySet<TEntity>() where TEntity : class, IEntity
    {
        return _dbContext.Set<TEntity>();
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}