using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;
using minipdv.Infrastructure.Data.Context;

namespace minipdv.Infrastructure.Data.Repositories;

public class ProdutoControladoRepository : Repository<ProdutoControlado>, IProdutoControladoRepository
{
    public ProdutoControladoRepository(MiniPDVContext context, ILogger<ProdutoControladoRepository> logger) : base(context, logger) { }

    public async Task<ProdutoControlado?> GetByRegistroMsAsync(string registroMs)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.RegistroMS == registroMs);
    }
}
