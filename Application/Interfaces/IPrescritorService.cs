using minipdv.Domain.Entities;

namespace minipdv.Application.Interfaces;

public interface IPrescritorService
{
    Task<IEnumerable<Prescritor>> GetAllAsync();
    Task<Prescritor?> GetByIdAsync(int id);
    Task<Prescritor> AddAsync(Prescritor entity);
    Task UpdateAsync(Prescritor entity);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
