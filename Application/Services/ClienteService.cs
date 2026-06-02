using FluentValidation;
using Microsoft.Extensions.Logging;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;

namespace minipdv.Application.Services;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _repository;
    private readonly IValidator<Cliente> _validator;
    private readonly ILogger<ClienteService> _logger;

    public ClienteService(IClienteRepository repository, IValidator<Cliente> validator, ILogger<ClienteService> logger)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
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
        var created = await _repository.AddAsync(entity);
        _logger.LogInformation("Cliente criado: Id={Id}, Nome={Nome}, CPF={Cpf}", created.Id, created.Nome, created.Cpf);
        return created;
    }

    public async Task UpdateAsync(Cliente entity)
    {
        await _validator.ValidateAndThrowAsync(entity);
        await _repository.UpdateAsync(entity);
        _logger.LogInformation("Cliente atualizado: Id={Id}, Nome={Nome}", entity.Id, entity.Nome);
    }

    public async Task DeleteAsync(int id)
    {
        if (!await _repository.ExistsAsync(id))
        {
            _logger.LogWarning("Tentativa de excluir cliente inexistente: Id={Id}", id);
        }
        await _repository.DeleteAsync(id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _repository.ExistsAsync(id);
    }
}
