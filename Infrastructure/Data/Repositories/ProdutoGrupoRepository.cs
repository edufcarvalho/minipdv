using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;
using minipdv.Infrastructure.Data.Context;

namespace minipdv.Infrastructure.Data.Repositories;

public class ProdutoGrupoRepository : Repository<ProdutoGrupo>, IProdutoGrupoRepository
{
    public ProdutoGrupoRepository(MiniPDVContext context) : base(context) { }
}
