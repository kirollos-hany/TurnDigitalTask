using Microsoft.EntityFrameworkCore;
using TurnDigital.Domain.DataAccess.Interfaces;
using TurnDigital.Domain.Entities.Interfaces;

namespace TurnDigital.Infrastructure.DataAccess;

internal class ReadRepository : IReadRepository
{
    private readonly TurnDigitalDbContext _dbContext;

    public ReadRepository(TurnDigitalDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IQueryable<TEntity> GetQueryable<TEntity>() where TEntity : class, IEntity
    {
        return _dbContext.Set<TEntity>().AsNoTrackingWithIdentityResolution();
    }
}