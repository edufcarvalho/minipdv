using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using minipdv.Domain.Entities.Base;
using minipdv.Domain.Interfaces;
using minipdv.Infrastructure.Data.Context;

namespace minipdv.Infrastructure.Data.Repositories;

public abstract class Repository<T> : IRepository<T> where T : Entity
{
    protected readonly MiniPDVContext _context;
    protected readonly DbSet<T> _dbSet;
    protected readonly ILogger _logger;

    protected Repository(MiniPDVContext context, ILogger logger)
    {
        _context = context;
        _dbSet = context.Set<T>();
        _logger = logger;
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        _logger.LogDebug("Buscando {EntityType} pelo Id={Id}", typeof(T).Name, id);
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        _logger.LogDebug("Buscando todos os registros de {EntityType}", typeof(T).Name);
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        entity.CriadoEm = DateTime.UtcNow;
        await _dbSet.AddAsync(entity);
        _logger.LogDebug("{EntityType} adicionado ao change tracker. Id={Id}", typeof(T).Name, entity.Id);
        return entity;
    }

    public virtual async Task UpdateAsync(T entity)
    {
        entity.AtualizadoEm = DateTime.UtcNow;
        _dbSet.Update(entity);
        _logger.LogDebug("{EntityType} marcado para atualização. Id={Id}", typeof(T).Name, entity.Id);
    }

    public virtual async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            _logger.LogDebug("{EntityType} marcado para remoção. Id={Id}", typeof(T).Name, id);
        }
        else
        {
            _logger.LogWarning("Tentativa de remover {EntityType} inexistente. Id={Id}", typeof(T).Name, id);
        }
    }

    public virtual async Task<bool> ExistsAsync(int id)
    {
        return await _dbSet.AnyAsync(e => e.Id == id);
    }
}
