using FluentValidation;
using Microsoft.Extensions.Logging;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;

namespace minipdv.Application.Services;

public class PrincipioAtivoService : CrudServiceBase<PrincipioAtivo, IPrincipioAtivoRepository>, IPrincipioAtivoService
{
    public PrincipioAtivoService(IPrincipioAtivoRepository repository, IValidator<PrincipioAtivo> validator, ILogger<PrincipioAtivoService> logger)
        : base(repository, validator, logger) { }

    protected override string EntityDisplayName => "PrincipioAtivo";

    public async Task<PrincipioAtivo?> GetByNomeAsync(string nome)
    {
        return await Repository.GetByNomeAsync(nome);
    }
}
