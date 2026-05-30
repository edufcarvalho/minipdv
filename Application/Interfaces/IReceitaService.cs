using minipdv.Domain.Entities;

namespace minipdv.Application.Interfaces;

public interface IReceitaService
{
    Task<IEnumerable<Receita>> GetAllAsync();
    Task<Receita?> GetByIdAsync(int id);
    Task<Receita> AddAsync(Receita entity);
    Task UpdateAsync(Receita entity);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
