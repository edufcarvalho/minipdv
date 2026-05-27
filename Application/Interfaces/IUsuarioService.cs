using minipdv.Domain.Entities;

namespace minipdv.Application.Interfaces;

public interface IUsuarioService
{
    Task<IEnumerable<Usuario>> GetAllAsync();
    Task<Usuario?> GetByIdAsync(int id);
    Task<Usuario?> GetByLoginAsync(string login);
    Task<Usuario> AddAsync(Usuario entity);
    Task UpdateAsync(Usuario entity);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
