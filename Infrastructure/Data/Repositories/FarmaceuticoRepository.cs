using Microsoft.EntityFrameworkCore;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;
using minipdv.Infrastructure.Data.Context;

namespace minipdv.Infrastructure.Data.Repositories;

public class FarmaceuticoRepository : Repository<Farmaceutico>, IFarmaceuticoRepository
{
    public FarmaceuticoRepository(MiniPDVContext context) : base(context) { }

    public async Task<Farmaceutico?> GetByCrfAsync(string crf)
    {
        return await _dbSet.FirstOrDefaultAsync(f => f.Crf == crf);
    }
}
