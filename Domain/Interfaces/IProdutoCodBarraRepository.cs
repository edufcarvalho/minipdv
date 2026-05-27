using minipdv.Domain.Entities;

namespace minipdv.Domain.Interfaces;

public interface IProdutoCodBarraRepository
{
    Task<IEnumerable<ProdutoCodBarra>> GetAllAsync();
    Task<IEnumerable<ProdutoCodBarra>> GetByProdutoIdAsync(int produtoId);
    Task<ProdutoCodBarra?> GetByCodBarraAsync(int codBarra);
    Task<ProdutoCodBarra> AddAsync(ProdutoCodBarra entity);
    Task UpdateAsync(ProdutoCodBarra entity);
    Task DeleteAsync(int codBarra);
    Task<bool> ExistsAsync(int codBarra);
}
