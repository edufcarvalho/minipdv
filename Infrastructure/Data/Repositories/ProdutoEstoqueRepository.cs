using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;
using minipdv.Infrastructure.Data.Context;

namespace minipdv.Infrastructure.Data.Repositories;

public class ProdutoEstoqueRepository : IProdutoEstoqueRepository
{
    private readonly MiniPDVContext _context;
    private readonly DbSet<ProdutoEstoque> _dbSet;
    private readonly ILogger<ProdutoEstoqueRepository> _logger;

    public ProdutoEstoqueRepository(MiniPDVContext context, ILogger<ProdutoEstoqueRepository> logger)
    {
        _context = context;
        _dbSet = context.Set<ProdutoEstoque>();
        _logger = logger;
    }

    public async Task<IEnumerable<ProdutoEstoque>> GetAllAsync()
    {
        return await _dbSet.Include(e => e.Produto).ToListAsync();
    }

    public async Task<IEnumerable<ProdutoEstoque>> GetByProdutoIdAsync(int produtoId)
    {
        return await _dbSet.Where(e => e.ProdutoId == produtoId).ToListAsync();
    }

    public async Task<ProdutoEstoque?> GetByIdAsync(int produtoId, string lote)
    {
        return await _dbSet.FindAsync(produtoId, lote);
    }

    public async Task<ProdutoEstoque> AddAsync(ProdutoEstoque entity)
    {
        await _dbSet.AddAsync(entity);
        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("ProdutoEstoque criado. ProdutoId={ProdutoId}, Lote={Lote}, Qtd={Quantidade}", entity.ProdutoId, entity.Lote, entity.Quantidade);
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao criar ProdutoEstoque ProdutoId={ProdutoId} Lote={Lote}", entity.ProdutoId, entity.Lote);
            throw;
        }
    }

    public async Task UpdateAsync(ProdutoEstoque entity)
    {
        _dbSet.Update(entity);
        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("ProdutoEstoque atualizado. ProdutoId={ProdutoId}, Lote={Lote}, Qtd={Quantidade}", entity.ProdutoId, entity.Lote, entity.Quantidade);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao atualizar ProdutoEstoque ProdutoId={ProdutoId} Lote={Lote}", entity.ProdutoId, entity.Lote);
            throw;
        }
    }

    public async Task DeleteAsync(int produtoId, string lote)
    {
        var entity = await GetByIdAsync(produtoId, lote);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            _logger.LogInformation("ProdutoEstoque removido. ProdutoId={ProdutoId}, Lote={Lote}", produtoId, lote);
        }
        else
        {
            _logger.LogWarning("Tentativa de remover ProdutoEstoque inexistente. ProdutoId={ProdutoId}, Lote={Lote}", produtoId, lote);
        }
    }

    public async Task<bool> ExistsAsync(int produtoId, string lote)
    {
        return await _dbSet.AnyAsync(e => e.ProdutoId == produtoId && e.Lote == lote);
    }
}
