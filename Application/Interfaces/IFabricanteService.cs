using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;

namespace minipdv.Application.Interfaces;

public interface IFabricanteService : ICrudService<Fabricante>
{
    Task<Fabricante?> GetByCnpjAsync(string cnpj);
}
