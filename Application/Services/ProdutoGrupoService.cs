using FluentValidation;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;

namespace minipdv.Application.Services;

public class ProdutoGrupoService : IProdutoGrupoService
{
    private readonly IProdutoGrupoRepository _repository;
    private readonly IValidator<ProdutoGrupo> _validator;

    public ProdutoGrupoService(IProdutoGrupoRepository repository, IValidator<ProdutoGrupo> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<IEnumerable<ProdutoGrupo>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<ProdutoGrupo?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<ProdutoGrupo> AddAsync(ProdutoGrupo entity)
    {
        await _validator.ValidateAndThrowAsync(entity);
        return await _repository.AddAsync(entity);
    }

    public async Task UpdateAsync(ProdutoGrupo entity)
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
