using minipdv.Domain.Entities;

namespace minipdv.Application.Interfaces;

public interface IProdutoService
{
    Task<IEnumerable<Produto>> GetAllAsync();
    Task<Produto?> GetByIdAsync(int id);
    Task<Produto?> GetByCodBarraAsync(int codBarra);
    Task<Produto> AddAsync(Produto entity);
    Task UpdateAsync(Produto entity);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
