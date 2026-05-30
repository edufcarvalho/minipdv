using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;
using minipdv.Infrastructure.Data.Context;

namespace minipdv.Infrastructure.Data.Repositories;

public class PrescritorRepository : Repository<Prescritor>, IPrescritorRepository
{
    public PrescritorRepository(MiniPDVContext context) : base(context) { }
}
