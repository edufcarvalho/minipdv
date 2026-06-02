using FluentValidation;
using Microsoft.Extensions.Logging;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;

namespace minipdv.Application.Services;

public class ClienteService : CrudServiceBase<Cliente, IClienteRepository>, IClienteService
{
    public ClienteService(IClienteRepository repository, IValidator<Cliente> validator, ILogger<ClienteService> logger)
        : base(repository, validator, logger) { }

    protected override string EntityDisplayName => "Cliente";
}
