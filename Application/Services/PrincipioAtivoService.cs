using FluentValidation;
using Microsoft.Extensions.Logging;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;

namespace minipdv.Application.Services;

public class PrincipioAtivoService : IPrincipioAtivoService
{
    private readonly IPrincipioAtivoRepository _repository;
    private readonly IValidator<PrincipioAtivo> _validator;
    private readonly ILogger<PrincipioAtivoService> _logger;

    public PrincipioAtivoService(IPrincipioAtivoRepository repository, IValidator<PrincipioAtivo> validator, ILogger<PrincipioAtivoService> logger)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
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
        var created = await _repository.AddAsync(entity);
        _logger.LogInformation("PrincipioAtivo criado: Id={Id}, Nome={Nome}", created.Id, created.Nome);
        return created;
    }

    public async Task UpdateAsync(PrincipioAtivo entity)
    {
        await _validator.ValidateAndThrowAsync(entity);
        await _repository.UpdateAsync(entity);
        _logger.LogInformation("PrincipioAtivo atualizado: Id={Id}, Nome={Nome}", entity.Id, entity.Nome);
    }

    public async Task DeleteAsync(int id)
    {
        if (!await _repository.ExistsAsync(id))
        {
            _logger.LogWarning("Tentativa de excluir PrincipioAtivo inexistente: Id={Id}", id);
        }
        await _repository.DeleteAsync(id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _repository.ExistsAsync(id);
    }
}
