using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;

namespace minipdv.Application.Interfaces;

public interface IProdutoService : ICrudService<Produto>
{
    Task<Produto?> GetByCodBarraAsync(int codBarra);
}
