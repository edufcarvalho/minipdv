using FluentValidation;
using Microsoft.Extensions.Logging;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;

namespace minipdv.Application.Services;

public class AdministradorService : IAdministradorService
{
    private readonly IAdministradorRepository _repository;
    private readonly IValidator<Administrador> _validator;
    private readonly ILogger<AdministradorService> _logger;

    public AdministradorService(IAdministradorRepository repository, IValidator<Administrador> validator, ILogger<AdministradorService> logger)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
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
        var created = await _repository.AddAsync(entity);
        _logger.LogInformation("Administrador criado: Id={Id}, Nome={Nome}, Login={Login}", created.Id, created.Nome, created.Login);
        return created;
    }

    public async Task UpdateAsync(Administrador entity)
    {
        await _validator.ValidateAndThrowAsync(entity);
        await _repository.UpdateAsync(entity);
        _logger.LogInformation("Administrador atualizado: Id={Id}, Nome={Nome}", entity.Id, entity.Nome);
    }

    public async Task DeleteAsync(int id)
    {
        if (!await _repository.ExistsAsync(id))
        {
            _logger.LogWarning("Tentativa de excluir administrador inexistente: Id={Id}", id);
        }
        await _repository.DeleteAsync(id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _repository.ExistsAsync(id);
    }
}
