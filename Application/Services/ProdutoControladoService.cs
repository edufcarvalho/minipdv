using FluentValidation;
using Microsoft.Extensions.Logging;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;

namespace minipdv.Application.Services;

public class ProdutoControladoService : CrudServiceBase<ProdutoControlado, IProdutoControladoRepository>, IProdutoControladoService
{
    public ProdutoControladoService(IProdutoControladoRepository repository, IValidator<ProdutoControlado> validator, ILogger<ProdutoControladoService> logger)
        : base(repository, validator, logger) { }

    protected override string EntityDisplayName => "ProdutoControlado";

    public async Task<ProdutoControlado?> GetByRegistroMsAsync(string registroMs)
    {
        return await Repository.GetByRegistroMsAsync(registroMs);
    }
}
