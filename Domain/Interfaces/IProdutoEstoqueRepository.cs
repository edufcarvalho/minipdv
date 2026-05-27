using minipdv.Domain.Entities;

namespace minipdv.Domain.Interfaces;

public interface IProdutoEstoqueRepository
{
    Task<IEnumerable<ProdutoEstoque>> GetAllAsync();
    Task<IEnumerable<ProdutoEstoque>> GetByProdutoIdAsync(int produtoId);
    Task<ProdutoEstoque?> GetByIdAsync(int produtoId, string lote);
    Task<ProdutoEstoque> AddAsync(ProdutoEstoque entity);
    Task UpdateAsync(ProdutoEstoque entity);
    Task DeleteAsync(int produtoId, string lote);
    Task<bool> ExistsAsync(int produtoId, string lote);
}
