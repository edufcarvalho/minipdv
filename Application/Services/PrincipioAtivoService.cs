using FluentValidation;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;

namespace minipdv.Application.Services;

public class PrincipioAtivoService : IPrincipioAtivoService
{
    private readonly IPrincipioAtivoRepository _repository;
    private readonly IValidator<PrincipioAtivo> _validator;

    public PrincipioAtivoService(IPrincipioAtivoRepository repository, IValidator<PrincipioAtivo> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<IEnumerable<PrincipioAtivo>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<PrincipioAtivo?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<PrincipioAtivo?> GetByNomeAsync(string nome)
    {
        return await _repository.GetByNomeAsync(nome);
    }

    public async Task<PrincipioAtivo> AddAsync(PrincipioAtivo entity)
    {
        await _validator.ValidateAndThrowAsync(entity);
        return await _repository.AddAsync(entity);
    }

    public async Task UpdateAsync(PrincipioAtivo entity)
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
