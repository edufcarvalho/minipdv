using FluentValidation;
using Microsoft.Extensions.Logging;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;

namespace minipdv.Application.Services;

public class PrescritorService : IPrescritorService
{
    private readonly IPrescritorRepository _repository;
    private readonly IValidator<Prescritor> _validator;
    private readonly ILogger<PrescritorService> _logger;

    public PrescritorService(IPrescritorRepository repository, IValidator<Prescritor> validator, ILogger<PrescritorService> logger)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<IEnumerable<Prescritor>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Prescritor?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Prescritor> AddAsync(Prescritor entity)
    {
        await _validator.ValidateAndThrowAsync(entity);
        var created = await _repository.AddAsync(entity);
        _logger.LogInformation("Prescritor criado: Id={Id}, Nome={Nome}, Conselho={Conselho}", created.Id, created.Nome, created.Conselho);
        return created;
    }

    public async Task UpdateAsync(Prescritor entity)
    {
        await _validator.ValidateAndThrowAsync(entity);
        await _repository.UpdateAsync(entity);
        _logger.LogInformation("Prescritor atualizado: Id={Id}, Nome={Nome}", entity.Id, entity.Nome);
    }

    public async Task DeleteAsync(int id)
    {
        if (!await _repository.ExistsAsync(id))
        {
            _logger.LogWarning("Tentativa de excluir prescritor inexistente: Id={Id}", id);
        }
        await _repository.DeleteAsync(id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _repository.ExistsAsync(id);
    }
}
