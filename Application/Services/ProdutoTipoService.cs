using FluentValidation;
using Microsoft.Extensions.Logging;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;

namespace minipdv.Application.Services;

public class ProdutoTipoService : IProdutoTipoService
{
    private readonly IProdutoTipoRepository _repository;
    private readonly IValidator<ProdutoTipo> _validator;
    private readonly ILogger<ProdutoTipoService> _logger;

    public ProdutoTipoService(IProdutoTipoRepository repository, IValidator<ProdutoTipo> validator, ILogger<ProdutoTipoService> logger)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<IEnumerable<ProdutoTipo>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<ProdutoTipo?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<ProdutoTipo> AddAsync(ProdutoTipo entity)
    {
        await _validator.ValidateAndThrowAsync(entity);
        var created = await _repository.AddAsync(entity);
        _logger.LogInformation("ProdutoTipo criado: Id={Id}, Nome={Nome}", created.Id, created.Nome);
        return created;
    }

    public async Task UpdateAsync(ProdutoTipo entity)
    {
        await _validator.ValidateAndThrowAsync(entity);
        await _repository.UpdateAsync(entity);
        _logger.LogInformation("ProdutoTipo atualizado: Id={Id}, Nome={Nome}", entity.Id, entity.Nome);
    }

    public async Task DeleteAsync(int id)
    {
        if (!await _repository.ExistsAsync(id))
        {
            _logger.LogWarning("Tentativa de excluir ProdutoTipo inexistente: Id={Id}", id);
        }
        await _repository.DeleteAsync(id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _repository.ExistsAsync(id);
    }
}
