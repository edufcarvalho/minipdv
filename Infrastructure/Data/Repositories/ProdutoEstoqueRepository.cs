using Microsoft.EntityFrameworkCore;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;
using minipdv.Infrastructure.Data.Context;

namespace minipdv.Infrastructure.Data.Repositories;

public class ProdutoEstoqueRepository : IProdutoEstoqueRepository
{
    private readonly MiniPDVContext _context;
    private readonly DbSet<ProdutoEstoque> _dbSet;

    public ProdutoEstoqueRepository(MiniPDVContext context)
    {
        _context = context;
        _dbSet = context.Set<ProdutoEstoque>();
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
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(ProdutoEstoque entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int produtoId, string lote)
    {
        var entity = await GetByIdAsync(produtoId, lote);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int produtoId, string lote)
    {
        return await _dbSet.AnyAsync(e => e.ProdutoId == produtoId && e.Lote == lote);
    }
}
