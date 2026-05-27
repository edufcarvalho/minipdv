using minipdv.Domain.Entities;

namespace minipdv.Application.Interfaces;

public interface IProdutoTipoService
{
    Task<IEnumerable<ProdutoTipo>> GetAllAsync();
    Task<ProdutoTipo?> GetByIdAsync(int id);
    Task<ProdutoTipo> AddAsync(ProdutoTipo entity);
    Task UpdateAsync(ProdutoTipo entity);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
