using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;

namespace minipdv.Application.Interfaces;

public interface IProdutoControladoService : ICrudService<ProdutoControlado>
{
    Task<ProdutoControlado?> GetByRegistroMsAsync(string registroMs);
}
