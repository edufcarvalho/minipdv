using FluentValidation;
using Microsoft.Extensions.Logging;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;

namespace minipdv.Application.Services;

public class PrescritorService : CrudServiceBase<Prescritor, IPrescritorRepository>, IPrescritorService
{
    public PrescritorService(IPrescritorRepository repository, IValidator<Prescritor> validator, ILogger<PrescritorService> logger)
        : base(repository, validator, logger) { }

    protected override string EntityDisplayName => "Prescritor";
}
