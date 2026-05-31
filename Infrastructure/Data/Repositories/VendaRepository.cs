using Microsoft.EntityFrameworkCore;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;
using minipdv.Infrastructure.Data.Context;

namespace minipdv.Infrastructure.Data.Repositories;

public class VendaRepository : Repository<Venda>, IVendaRepository
{
    public VendaRepository(MiniPDVContext context) : base(context) { }

    public override async Task<Venda?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(v => v.Vendedor)
            .Include(v => v.Cliente)
            .Include(v => v.VendaItens)
                .ThenInclude(vi => vi.Produto)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public override async Task<IEnumerable<Venda>> GetAllAsync()
    {
        return await _dbSet
            .Include(v => v.Vendedor)
            .Include(v => v.Cliente)
            .Include(v => v.VendaItens)
                .ThenInclude(vi => vi.Produto)
            .ToListAsync();
    }
}
