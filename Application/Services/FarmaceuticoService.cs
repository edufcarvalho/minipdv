using FluentValidation;
using Microsoft.Extensions.Logging;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;

namespace minipdv.Application.Services;

public class FarmaceuticoService : IFarmaceuticoService
{
    private readonly IFarmaceuticoRepository _repository;
    private readonly IValidator<Farmaceutico> _validator;
    private readonly ILogger<FarmaceuticoService> _logger;

    public FarmaceuticoService(IFarmaceuticoRepository repository, IValidator<Farmaceutico> validator, ILogger<FarmaceuticoService> logger)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<IEnumerable<Farmaceutico>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Farmaceutico?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Farmaceutico?> GetByCrfAsync(string crf)
    {
        return await _repository.GetByCrfAsync(crf);
    }

    public async Task<Farmaceutico> AddAsync(Farmaceutico entity)
    {
        await _validator.ValidateAndThrowAsync(entity);
        var created = await _repository.AddAsync(entity);
        _logger.LogInformation("Farmacêutico criado: Id={Id}, Nome={Nome}, CRF={Crf}", created.Id, created.Nome, created.Crf);
        return created;
    }

    public async Task UpdateAsync(Farmaceutico entity)
    {
        await _validator.ValidateAndThrowAsync(entity);
        await _repository.UpdateAsync(entity);
        _logger.LogInformation("Farmacêutico atualizado: Id={Id}, Nome={Nome}", entity.Id, entity.Nome);
    }

    public async Task DeleteAsync(int id)
    {
        if (!await _repository.ExistsAsync(id))
        {
            _logger.LogWarning("Tentativa de excluir farmacêutico inexistente: Id={Id}", id);
        }
        await _repository.DeleteAsync(id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _repository.ExistsAsync(id);
    }
}
