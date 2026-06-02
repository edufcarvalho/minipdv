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
        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("{EntityType} criado com sucesso. Id={Id}", typeof(T).Name, entity.Id);
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao criar {EntityType} no banco de dados", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task UpdateAsync(T entity)
    {
        entity.AtualizadoEm = DateTime.UtcNow;
        _dbSet.Update(entity);
        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("{EntityType} atualizado com sucesso. Id={Id}", typeof(T).Name, entity.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao atualizar {EntityType} Id={Id} no banco de dados", typeof(T).Name, entity.Id);
            throw;
        }
    }

    public virtual async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("{EntityType} removido com sucesso. Id={Id}", typeof(T).Name, id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao remover {EntityType} Id={Id} do banco de dados", typeof(T).Name, id);
                throw;
            }
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
