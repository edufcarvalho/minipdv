using FluentValidation;
using Microsoft.Extensions.Logging;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;

namespace minipdv.Application.Services;

public class ContatoService : IContatoService
{
    private readonly IContatoRepository _repository;
    private readonly IValidator<Contato> _validator;
    private readonly ILogger<ContatoService> _logger;

    public ContatoService(IContatoRepository repository, IValidator<Contato> validator, ILogger<ContatoService> logger)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
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
        var created = await _repository.AddAsync(entity);
        _logger.LogInformation("Contato criado: Id={Id}, Email={Email}, Telefone={Telefone}", created.Id, created.Email, created.Telefone);
        return created;
    }

    public async Task UpdateAsync(Contato entity)
    {
        await _validator.ValidateAndThrowAsync(entity);
        await _repository.UpdateAsync(entity);
        _logger.LogInformation("Contato atualizado: Id={Id}", entity.Id);
    }

    public async Task DeleteAsync(int id)
    {
        if (!await _repository.ExistsAsync(id))
        {
            _logger.LogWarning("Tentativa de excluir contato inexistente: Id={Id}", id);
        }
        await _repository.DeleteAsync(id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _repository.ExistsAsync(id);
    }
}
