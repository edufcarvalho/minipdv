using FluentValidation;
using Microsoft.Extensions.Logging;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;

namespace minipdv.Application.Services;

public class AdministradorService : CrudServiceBase<Administrador, IAdministradorRepository>, IAdministradorService
{
    public AdministradorService(IAdministradorRepository repository, IValidator<Administrador> validator, ILogger<AdministradorService> logger)
        : base(repository, validator, logger) { }

    protected override string EntityDisplayName => "Administrador";
}
