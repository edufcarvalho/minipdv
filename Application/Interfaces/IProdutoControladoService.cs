using minipdv.Domain.Entities;

namespace minipdv.Application.Interfaces;

public interface IProdutoControladoService
{
    Task<IEnumerable<ProdutoControlado>> GetAllAsync();
    Task<ProdutoControlado?> GetByIdAsync(int id);
    Task<ProdutoControlado?> GetByRegistroMsAsync(string registroMs);
    Task<ProdutoControlado> AddAsync(ProdutoControlado entity);
    Task UpdateAsync(ProdutoControlado entity);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
