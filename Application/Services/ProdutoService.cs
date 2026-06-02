using FluentValidation;
using Microsoft.Extensions.Logging;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;

namespace minipdv.Application.Services;

public class ProdutoService : CrudServiceBase<Produto, IProdutoRepository>, IProdutoService
{
    public ProdutoService(IProdutoRepository repository, IValidator<Produto> validator, ILogger<ProdutoService> logger)
        : base(repository, validator, logger) { }

    protected override string EntityDisplayName => "Produto";

    public async Task<Produto?> GetByCodBarraAsync(int codBarra)
    {
        return await Repository.GetByCodBarraAsync(codBarra);
    }
}
