using Microsoft.EntityFrameworkCore;
using TurnDigital.Domain.Entities.Interfaces;

namespace TurnDigital.Domain.DataAccess.Interfaces;

public interface IRepository
{
    DbSet<TEntity> GetEntitySet<TEntity>() where TEntity : class, IEntity;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}