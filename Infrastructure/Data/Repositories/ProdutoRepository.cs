using Microsoft.EntityFrameworkCore;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;
using minipdv.Infrastructure.Data.Context;

namespace minipdv.Infrastructure.Data.Repositories;

public class ProdutoRepository : Repository<Produto>, IProdutoRepository
{
    public ProdutoRepository(MiniPDVContext context) : base(context) { }

    public async Task<Produto?> GetByCodBarraAsync(int codBarra)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.CodBarra == codBarra);
    }
}
