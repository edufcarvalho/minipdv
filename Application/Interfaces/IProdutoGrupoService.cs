using minipdv.Domain.Entities;

namespace minipdv.Application.Interfaces;

public interface IProdutoGrupoService
{
    Task<IEnumerable<ProdutoGrupo>> GetAllAsync();
    Task<ProdutoGrupo?> GetByIdAsync(int id);
    Task<ProdutoGrupo> AddAsync(ProdutoGrupo entity);
    Task UpdateAsync(ProdutoGrupo entity);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
