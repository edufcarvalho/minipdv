using FluentValidation;
using Microsoft.Extensions.Logging;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;

namespace minipdv.Application.Services;

public class FarmaceuticoService : CrudServiceBase<Farmaceutico, IFarmaceuticoRepository>, IFarmaceuticoService
{
    public FarmaceuticoService(IFarmaceuticoRepository repository, IValidator<Farmaceutico> validator, ILogger<FarmaceuticoService> logger)
        : base(repository, validator, logger) { }

    protected override string EntityDisplayName => "Farmacêutico";

    public async Task<Farmaceutico?> GetByCrfAsync(string crf)
    {
        return await Repository.GetByCrfAsync(crf);
    }
}
