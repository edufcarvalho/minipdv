using minipdv.Domain.Entities;

namespace minipdv.Application.Interfaces;

public interface IAdministradorService
{
    Task<IEnumerable<Administrador>> GetAllAsync();
    Task<Administrador?> GetByIdAsync(int id);
    Task<Administrador> AddAsync(Administrador entity);
    Task UpdateAsync(Administrador entity);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
