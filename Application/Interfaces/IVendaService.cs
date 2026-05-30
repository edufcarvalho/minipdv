using minipdv.Domain.Entities;

namespace minipdv.Application.Interfaces;

public interface IVendaService
{
    Task<IEnumerable<Venda>> GetAllAsync();
    Task<Venda?> GetByIdAsync(int id);
    Task<Venda> AddAsync(Venda entity);
    Task DeleteAsync(int id);
}
