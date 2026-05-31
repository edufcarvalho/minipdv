using FluentValidation;
using Microsoft.EntityFrameworkCore;
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

    public ProdutoEstoqueService(
        IProdutoEstoqueRepository repository,
        MiniPDVContext context,
        IValidator<ProdutoEstoque> validator)
    {
        _repository = repository;
        _context = context;
        _validator = validator;
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
        return created;
    }

    public async Task UpdateAsync(ProdutoEstoque entity)
    {
        await _validator.ValidateAndThrowAsync(entity);
        await _repository.UpdateAsync(entity);
        await RecaleEstoqueAsync(entity.ProdutoId);
    }

    public async Task DeleteAsync(int produtoId, string lote)
    {
        await _repository.DeleteAsync(produtoId, lote);
        await RecaleEstoqueAsync(produtoId);
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

        await _context.Set<Produto>()
            .Where(p => p.Id == produtoId)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.Estoque, total));
    }
}
