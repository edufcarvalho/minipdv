using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;
using minipdv.Infrastructure.Data.Context;

namespace minipdv.Application.Services;

public class ProdutoEstoqueService : IProdutoEstoqueService
{
    private readonly IProdutoEstoqueRepository _repository;
    private readonly MiniPDVContext _context;
    private readonly IValidator<ProdutoEstoque> _validator;
    private readonly ILogger<ProdutoEstoqueService> _logger;

    public ProdutoEstoqueService(
        IProdutoEstoqueRepository repository,
        MiniPDVContext context,
        IValidator<ProdutoEstoque> validator,
        ILogger<ProdutoEstoqueService> logger)
    {
        _repository = repository;
        _context = context;
        _validator = validator;
        _logger = logger;
    }

    public async Task<IEnumerable<ProdutoEstoque>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<IEnumerable<ProdutoEstoque>> GetByProdutoIdAsync(int produtoId)
    {
        return await _repository.GetByProdutoIdAsync(produtoId);
    }

    public async Task<ProdutoEstoque?> GetByIdAsync(int produtoId, string lote)
    {
        return await _repository.GetByIdAsync(produtoId, lote);
    }

    public async Task<ProdutoEstoque> AddAsync(ProdutoEstoque entity)
    {
        await _validator.ValidateAndThrowAsync(entity);
        var created = await _repository.AddAsync(entity);
        await RecaleEstoqueAsync(entity.ProdutoId);
        _logger.LogInformation("Estoque adicionado: ProdutoId={ProdutoId}, Lote={Lote}, Qtd={Quantidade}, Validade={Validade}",
            entity.ProdutoId, entity.Lote, entity.Quantidade, entity.Validade);
        return created;
    }

    public async Task UpdateAsync(ProdutoEstoque entity)
    {
        await _validator.ValidateAndThrowAsync(entity);
        await _repository.UpdateAsync(entity);
        await RecaleEstoqueAsync(entity.ProdutoId);
        _logger.LogInformation("Estoque atualizado: ProdutoId={ProdutoId}, Lote={Lote}, Qtd={Quantidade}",
            entity.ProdutoId, entity.Lote, entity.Quantidade);
    }

    public async Task DeleteAsync(int produtoId, string lote)
    {
        await _repository.DeleteAsync(produtoId, lote);
        await RecaleEstoqueAsync(produtoId);
        _logger.LogInformation("Estoque removido: ProdutoId={ProdutoId}, Lote={Lote}", produtoId, lote);
    }

    public async Task<bool> ExistsAsync(int produtoId, string lote)
    {
        return await _repository.ExistsAsync(produtoId, lote);
    }

    private async Task RecaleEstoqueAsync(int produtoId)
    {
        var total = await _context.Set<ProdutoEstoque>()
            .Where(e => e.ProdutoId == produtoId)
            .SumAsync(e => e.Quantidade);

        var produto = await _context.Set<Produto>().FindAsync(produtoId);

        if (produto is null)
        {
            _logger.LogWarning("Produto não encontrado para recálculo de estoque: ProdutoId={ProdutoId}", produtoId);
            return;
        }

        produto.Estoque = total;

        _logger.LogDebug("Estoque do produto ProdutoId={ProdutoId} recalculado: Total={Total}", produtoId, total);
    }
}
