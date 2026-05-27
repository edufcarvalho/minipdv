using Microsoft.EntityFrameworkCore;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;
using minipdv.Infrastructure.Data.Context;

namespace minipdv.Infrastructure.Data.Repositories;

public class ProdutoCodBarraRepository : IProdutoCodBarraRepository
{
    private readonly MiniPDVContext _context;
    private readonly DbSet<ProdutoCodBarra> _dbSet;

    public ProdutoCodBarraRepository(MiniPDVContext context)
    {
        _context = context;
        _dbSet = context.Set<ProdutoCodBarra>();
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
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(ProdutoCodBarra entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int codBarra)
    {
        var entity = await GetByCodBarraAsync(codBarra);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int codBarra)
    {
        return await _dbSet.AnyAsync(b => b.CodBarra == codBarra);
    }
}
