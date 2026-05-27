using minipdv.Domain.Entities;

namespace minipdv.Domain.Interfaces;

public interface IFarmaceuticoRepository : IRepository<Farmaceutico>
{
    Task<Farmaceutico?> GetByCrfAsync(string crf);
}
