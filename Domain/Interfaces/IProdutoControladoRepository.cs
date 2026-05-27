using minipdv.Domain.Entities;

namespace minipdv.Domain.Interfaces;

public interface IProdutoControladoRepository : IRepository<ProdutoControlado>
{
    Task<ProdutoControlado?> GetByRegistroMsAsync(string registroMs);
}
