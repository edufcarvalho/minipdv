using minipdv.Domain.Entities;

namespace minipdv.Application.Interfaces;

public interface IContatoService
{
    Task<IEnumerable<Contato>> GetAllAsync();
    Task<Contato?> GetByIdAsync(int id);
    Task<Contato> AddAsync(Contato entity);
    Task UpdateAsync(Contato entity);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
