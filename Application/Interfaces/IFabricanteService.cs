using minipdv.Domain.Entities;

namespace minipdv.Application.Interfaces;

public interface IFabricanteService
{
    Task<IEnumerable<Fabricante>> GetAllAsync();
    Task<Fabricante?> GetByIdAsync(int id);
    Task<Fabricante?> GetByCnpjAsync(string cnpj);
    Task<Fabricante> AddAsync(Fabricante entity);
    Task UpdateAsync(Fabricante entity);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
