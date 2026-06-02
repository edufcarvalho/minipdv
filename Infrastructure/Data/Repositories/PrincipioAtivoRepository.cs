using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;
using minipdv.Infrastructure.Data.Context;

namespace minipdv.Infrastructure.Data.Repositories;

public class PrincipioAtivoRepository : Repository<PrincipioAtivo>, IPrincipioAtivoRepository
{
    public PrincipioAtivoRepository(MiniPDVContext context, ILogger<PrincipioAtivoRepository> logger) : base(context, logger) { }

    public async Task<PrincipioAtivo?> GetByNomeAsync(string nome)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.Nome == nome);
    }
}
