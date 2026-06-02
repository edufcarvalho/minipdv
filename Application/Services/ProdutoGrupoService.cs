using FluentValidation;
using Microsoft.Extensions.Logging;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;

namespace minipdv.Application.Services;

public class ProdutoGrupoService : CrudServiceBase<ProdutoGrupo, IProdutoGrupoRepository>, IProdutoGrupoService
{
    public ProdutoGrupoService(IProdutoGrupoRepository repository, IValidator<ProdutoGrupo> validator, ILogger<ProdutoGrupoService> logger)
        : base(repository, validator, logger) { }

    protected override string EntityDisplayName => "ProdutoGrupo";
}
