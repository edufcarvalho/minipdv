using minipdv.Domain.Entities.Base;

namespace minipdv.Application.Interfaces;

public interface ICrudService<TEntity> where TEntity : Entity
{
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<TEntity?> GetByIdAsync(int id);
    Task<TEntity> AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
