using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;
using minipdv.Infrastructure.Data.Context;

namespace minipdv.Infrastructure.Data.Repositories;

public class ProdutoCodBarraRepository : IProdutoCodBarraRepository
{
    private readonly MiniPDVContext _context;
    private readonly DbSet<ProdutoCodBarra> _dbSet;
    private readonly ILogger<ProdutoCodBarraRepository> _logger;

    public ProdutoCodBarraRepository(MiniPDVContext context, ILogger<ProdutoCodBarraRepository> logger)
    {
        _context = context;
        _dbSet = context.Set<ProdutoCodBarra>();
        _logger = logger;
    }

    public async Task<IEnumerable<ProdutoCodBarra>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<IEnumerable<ProdutoCodBarra>> GetByProdutoIdAsync(int produtoId)
    {
        return await _dbSet.Where(b => b.ProdutoId == produtoId).ToListAsync();
    }

    public async Task<ProdutoCodBarra?> GetByCodBarraAsync(int codBarra)
    {
        return await _dbSet.FindAsync(codBarra);
    }

    public async Task<ProdutoCodBarra> AddAsync(ProdutoCodBarra entity)
    {
        entity.CriadoEm = DateTime.UtcNow;
        await _dbSet.AddAsync(entity);
        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("ProdutoCodBarra criado. CodBarra={CodBarra}, ProdutoId={ProdutoId}", entity.CodBarra, entity.ProdutoId);
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao criar ProdutoCodBarra CodBarra={CodBarra}", entity.CodBarra);
            throw;
        }
    }

    public async Task UpdateAsync(ProdutoCodBarra entity)
    {
        _dbSet.Update(entity);
        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("ProdutoCodBarra atualizado. CodBarra={CodBarra}", entity.CodBarra);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao atualizar ProdutoCodBarra CodBarra={CodBarra}", entity.CodBarra);
            throw;
        }
    }

    public async Task DeleteAsync(int codBarra)
    {
        var entity = await GetByCodBarraAsync(codBarra);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            _logger.LogInformation("ProdutoCodBarra removido. CodBarra={CodBarra}", codBarra);
        }
        else
        {
            _logger.LogWarning("Tentativa de remover ProdutoCodBarra inexistente. CodBarra={CodBarra}", codBarra);
        }
    }

    public async Task<bool> ExistsAsync(int codBarra)
    {
        return await _dbSet.AnyAsync(b => b.CodBarra == codBarra);
    }
}
