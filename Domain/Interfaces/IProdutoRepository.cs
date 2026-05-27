using minipdv.Domain.Entities;

namespace minipdv.Domain.Interfaces;

public interface IProdutoRepository : IRepository<Produto>
{
    Task<Produto?> GetByCodBarraAsync(int codBarra);
}
