using FluentValidation;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;

namespace minipdv.Application.Services;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _repository;
    private readonly IValidator<Cliente> _validator;

    public ClienteService(IClienteRepository repository, IValidator<Cliente> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<IEnumerable<Cliente>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Cliente?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Cliente> AddAsync(Cliente entity)
    {
        await _validator.ValidateAndThrowAsync(entity);
        return await _repository.AddAsync(entity);
    }

    public async Task UpdateAsync(Cliente entity)
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
