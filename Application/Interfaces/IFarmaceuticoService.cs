using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;

namespace minipdv.Application.Interfaces;

public interface IFarmaceuticoService : ICrudService<Farmaceutico>
{
    Task<Farmaceutico?> GetByCrfAsync(string crf);
}
