using FluentValidation;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;

namespace minipdv.Application.Services;

public class ProdutoEstoqueService : IProdutoEstoqueService
{
    private readonly IProdutoEstoqueRepository _repository;
    private readonly IValidator<ProdutoEstoque> _validator;

    public ProdutoEstoqueService(IProdutoEstoqueRepository repository, IValidator<ProdutoEstoque> validator)
    {
        _repository = repository;
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
        return await _repository.AddAsync(entity);
    }

    public async Task UpdateAsync(ProdutoEstoque entity)
    {
        await _validator.ValidateAndThrowAsync(entity);
        await _repository.UpdateAsync(entity);
    }

    public async Task DeleteAsync(int produtoId, string lote)
    {
        await _repository.DeleteAsync(produtoId, lote);
    }

    public async Task<bool> ExistsAsync(int produtoId, string lote)
    {
        return await _repository.ExistsAsync(produtoId, lote);
    }
}
