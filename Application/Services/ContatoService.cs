using FluentValidation;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;

namespace minipdv.Application.Services;

public class ContatoService : IContatoService
{
    private readonly IContatoRepository _repository;
    private readonly IValidator<Contato> _validator;

    public ContatoService(IContatoRepository repository, IValidator<Contato> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<IEnumerable<Contato>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Contato?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Contato> AddAsync(Contato entity)
    {
        await _validator.ValidateAndThrowAsync(entity);
        return await _repository.AddAsync(entity);
    }

    public async Task UpdateAsync(Contato entity)
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
