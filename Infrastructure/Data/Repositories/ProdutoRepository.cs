using Microsoft.EntityFrameworkCore;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;
using minipdv.Infrastructure.Data.Context;

namespace minipdv.Infrastructure.Data.Repositories;

public class ProdutoRepository : Repository<Produto>, IProdutoRepository
{
    public ProdutoRepository(MiniPDVContext context) : base(context) { }

    public override async Task<Produto?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(p => p.Estoques)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public override async Task<IEnumerable<Produto>> GetAllAsync()
    {
        return await _dbSet
            .Include(p => p.Estoques)
            .Include(p => p.Grupo)
            .ToListAsync();
    }

    public async Task<Produto?> GetByCodBarraAsync(int codBarra)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.CodBarra == codBarra);
    }
}
