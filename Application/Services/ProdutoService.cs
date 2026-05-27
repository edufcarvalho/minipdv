using FluentValidation;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;

namespace minipdv.Application.Services;

public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _repository;
    private readonly IValidator<Produto> _validator;

    public ProdutoService(IProdutoRepository repository, IValidator<Produto> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<IEnumerable<Produto>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Produto?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Produto?> GetByCodBarraAsync(int codBarra)
    {
        return await _repository.GetByCodBarraAsync(codBarra);
    }

    public async Task<Produto> AddAsync(Produto entity)
    {
        await _validator.ValidateAndThrowAsync(entity);
        return await _repository.AddAsync(entity);
    }

    public async Task UpdateAsync(Produto entity)
    {
        await _validator.ValidateAndThrowAsync(entity);
        await _repository.UpdateAsync(entity);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _repository.ExistsAsync(id);
    }
}
