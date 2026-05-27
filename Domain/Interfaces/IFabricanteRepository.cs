using minipdv.Domain.Entities;

namespace minipdv.Domain.Interfaces;

public interface IFabricanteRepository : IRepository<Fabricante>
{
    Task<Fabricante?> GetByCnpjAsync(string cnpj);
}
