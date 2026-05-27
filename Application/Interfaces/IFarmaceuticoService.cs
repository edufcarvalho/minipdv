using minipdv.Domain.Entities;

namespace minipdv.Application.Interfaces;

public interface IFarmaceuticoService
{
    Task<IEnumerable<Farmaceutico>> GetAllAsync();
    Task<Farmaceutico?> GetByIdAsync(int id);
    Task<Farmaceutico?> GetByCrfAsync(string crf);
    Task<Farmaceutico> AddAsync(Farmaceutico entity);
    Task UpdateAsync(Farmaceutico entity);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
