using minipdv.Domain.Entities;

namespace minipdv.Application.Interfaces;

public interface IVendaService
{
    Task<IEnumerable<Venda>> GetAllAsync();
    Task<Venda?> GetByIdAsync(int id);
    Task<Venda> AddAsync(Venda entity, List<int>? receitaIds = null);
    Task DeleteAsync(int id);
}
