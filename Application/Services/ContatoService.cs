using FluentValidation;
using Microsoft.Extensions.Logging;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;

namespace minipdv.Application.Services;

public class ContatoService : CrudServiceBase<Contato, IContatoRepository>, IContatoService
{
    public ContatoService(IContatoRepository repository, IValidator<Contato> validator, ILogger<ContatoService> logger)
        : base(repository, validator, logger) { }

    protected override string EntityDisplayName => "Contato";
}
