using FluentValidation;
using Microsoft.Extensions.Logging;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;

namespace minipdv.Application.Services;

public class ProdutoTipoService : CrudServiceBase<ProdutoTipo, IProdutoTipoRepository>, IProdutoTipoService
{
    public ProdutoTipoService(IProdutoTipoRepository repository, IValidator<ProdutoTipo> validator, ILogger<ProdutoTipoService> logger)
        : base(repository, validator, logger) { }

    protected override string EntityDisplayName => "ProdutoTipo";
}
