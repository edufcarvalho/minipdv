using FluentValidation;
using Microsoft.Extensions.Logging;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;

namespace minipdv.Application.Services;

public class FabricanteService : IFabricanteService
{
    private readonly IFabricanteRepository _repository;
    private readonly IValidator<Fabricante> _validator;
    private readonly ILogger<FabricanteService> _logger;

    public FabricanteService(IFabricanteRepository repository, IValidator<Fabricante> validator, ILogger<FabricanteService> logger)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<IEnumerable<Fabricante>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Fabricante?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Fabricante?> GetByCnpjAsync(string cnpj)
    {
        return await _repository.GetByCnpjAsync(cnpj);
    }

    public async Task<Fabricante> AddAsync(Fabricante entity)
    {
        await _validator.ValidateAndThrowAsync(entity);
        var created = await _repository.AddAsync(entity);
        _logger.LogInformation("Fabricante criado: Id={Id}, NomeFantasia={NomeFantasia}, CNPJ={Cnpj}", created.Id, created.NomeFantasia, created.Cnpj);
        return created;
    }

    public async Task UpdateAsync(Fabricante entity)
    {
        await _validator.ValidateAndThrowAsync(entity);
        await _repository.UpdateAsync(entity);
        _logger.LogInformation("Fabricante atualizado: Id={Id}, NomeFantasia={NomeFantasia}", entity.Id, entity.NomeFantasia);
    }

    public async Task DeleteAsync(int id)
    {
        if (!await _repository.ExistsAsync(id))
        {
            _logger.LogWarning("Tentativa de excluir fabricante inexistente: Id={Id}", id);
        }
        await _repository.DeleteAsync(id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _repository.ExistsAsync(id);
    }
}
