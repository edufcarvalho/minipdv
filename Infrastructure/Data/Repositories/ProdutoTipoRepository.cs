using Microsoft.Extensions.Logging;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;
using minipdv.Infrastructure.Data.Context;

namespace minipdv.Infrastructure.Data.Repositories;

public class ProdutoTipoRepository : Repository<ProdutoTipo>, IProdutoTipoRepository
{
    public ProdutoTipoRepository(MiniPDVContext context, ILogger<ProdutoTipoRepository> logger) : base(context, logger) { }
}
