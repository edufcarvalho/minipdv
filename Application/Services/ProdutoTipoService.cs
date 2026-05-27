using FluentValidation;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;

namespace minipdv.Application.Services;

public class ProdutoTipoService : IProdutoTipoService
{
    private readonly IProdutoTipoRepository _repository;
    private readonly IValidator<ProdutoTipo> _validator;

    public ProdutoTipoService(IProdutoTipoRepository repository, IValidator<ProdutoTipo> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<IEnumerable<ProdutoTipo>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<ProdutoTipo?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<ProdutoTipo> AddAsync(ProdutoTipo entity)
    {
        await _validator.ValidateAndThrowAsync(entity);
        return await _repository.AddAsync(entity);
    }

    public async Task UpdateAsync(ProdutoTipo entity)
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
