using FluentValidation;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;

namespace minipdv.Application.Services;

public class ProdutoControladoService : IProdutoControladoService
{
    private readonly IProdutoControladoRepository _repository;
    private readonly IValidator<ProdutoControlado> _validator;

    public ProdutoControladoService(IProdutoControladoRepository repository, IValidator<ProdutoControlado> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<IEnumerable<ProdutoControlado>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<ProdutoControlado?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<ProdutoControlado?> GetByRegistroMsAsync(string registroMs)
    {
        return await _repository.GetByRegistroMsAsync(registroMs);
    }

    public async Task<ProdutoControlado> AddAsync(ProdutoControlado entity)
    {
        await _validator.ValidateAndThrowAsync(entity);
        return await _repository.AddAsync(entity);
    }

    public async Task UpdateAsync(ProdutoControlado entity)
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
