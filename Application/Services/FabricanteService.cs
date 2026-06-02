using FluentValidation;
using Microsoft.Extensions.Logging;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;

namespace minipdv.Application.Services;

public class FabricanteService : CrudServiceBase<Fabricante, IFabricanteRepository>, IFabricanteService
{
    public FabricanteService(IFabricanteRepository repository, IValidator<Fabricante> validator, ILogger<FabricanteService> logger)
        : base(repository, validator, logger) { }

    protected override string EntityDisplayName => "Fabricante";

    public async Task<Fabricante?> GetByCnpjAsync(string cnpj)
    {
        return await Repository.GetByCnpjAsync(cnpj);
    }
}
