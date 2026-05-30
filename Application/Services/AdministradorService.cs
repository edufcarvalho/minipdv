using FluentValidation;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;

namespace minipdv.Application.Services;

public class AdministradorService : IAdministradorService
{
    private readonly IAdministradorRepository _repository;
    private readonly IValidator<Administrador> _validator;

    public AdministradorService(IAdministradorRepository repository, IValidator<Administrador> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<IEnumerable<Administrador>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Administrador?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Administrador> AddAsync(Administrador entity)
    {
        await _validator.ValidateAndThrowAsync(entity);
        return await _repository.AddAsync(entity);
    }

    public async Task UpdateAsync(Administrador entity)
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
