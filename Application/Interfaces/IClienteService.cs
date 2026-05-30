using minipdv.Domain.Entities;

namespace minipdv.Application.Interfaces;

public interface IClienteService
{
    Task<IEnumerable<Cliente>> GetAllAsync();
    Task<Cliente?> GetByIdAsync(int id);
    Task<Cliente> AddAsync(Cliente entity);
    Task UpdateAsync(Cliente entity);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
