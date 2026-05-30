using Microsoft.EntityFrameworkCore;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;
using minipdv.Infrastructure.Data.Context;

namespace minipdv.Infrastructure.Data.Repositories;

public class ReceitaRepository : Repository<Receita>, IReceitaRepository
{
    public ReceitaRepository(MiniPDVContext context) : base(context) { }

    public override async Task<Receita?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(r => r.Prescritor)
            .Include(r => r.Paciente)
            .Include(r => r.Comprador)
            .Include(r => r.ReceitaProdutoEstoques)
                .ThenInclude(rpe => rpe.ProdutoEstoque)
                    .ThenInclude(pe => pe.Produto)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public override async Task<IEnumerable<Receita>> GetAllAsync()
    {
        return await _dbSet
            .Include(r => r.Prescritor)
            .Include(r => r.Paciente)
            .Include(r => r.Comprador)
            .Include(r => r.ReceitaProdutoEstoques)
                .ThenInclude(rpe => rpe.ProdutoEstoque)
                    .ThenInclude(pe => pe.Produto)
            .ToListAsync();
    }
}
