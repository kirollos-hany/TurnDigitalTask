using TurnDigital.Domain.Entities.Interfaces;

namespace TurnDigital.Domain.DataAccess.Interfaces;

public interface IReadRepository
{
    IQueryable<TEntity> GetQueryable<TEntity>() where TEntity : class, IEntity;
}