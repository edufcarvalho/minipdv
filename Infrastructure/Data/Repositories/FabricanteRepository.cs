using Microsoft.EntityFrameworkCore;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;
using minipdv.Infrastructure.Data.Context;

namespace minipdv.Infrastructure.Data.Repositories;

public class FabricanteRepository : Repository<Fabricante>, IFabricanteRepository
{
    public FabricanteRepository(MiniPDVContext context) : base(context) { }

    public async Task<Fabricante?> GetByCnpjAsync(string cnpj)
    {
        return await _dbSet.FirstOrDefaultAsync(f => f.Cnpj == cnpj);
    }
}
