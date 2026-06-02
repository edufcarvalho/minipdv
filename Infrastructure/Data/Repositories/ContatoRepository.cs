using Microsoft.Extensions.Logging;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;
using minipdv.Infrastructure.Data.Context;

namespace minipdv.Infrastructure.Data.Repositories;

public class ContatoRepository : Repository<Contato>, IContatoRepository
{
    public ContatoRepository(MiniPDVContext context, ILogger<ContatoRepository> logger) : base(context, logger) { }
}
