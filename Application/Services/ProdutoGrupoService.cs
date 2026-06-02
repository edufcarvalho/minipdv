using FluentValidation;
using Microsoft.Extensions.Logging;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;

namespace minipdv.Application.Services;

public class ProdutoGrupoService : IProdutoGrupoService
{
    private readonly IProdutoGrupoRepository _repository;
    private readonly IValidator<ProdutoGrupo> _validator;
    private readonly ILogger<ProdutoGrupoService> _logger;

    public ProdutoGrupoService(IProdutoGrupoRepository repository, IValidator<ProdutoGrupo> validator, ILogger<ProdutoGrupoService> logger)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<IEnumerable<ProdutoGrupo>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<ProdutoGrupo?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<ProdutoGrupo> AddAsync(ProdutoGrupo entity)
    {
        await _validator.ValidateAndThrowAsync(entity);
        var created = await _repository.AddAsync(entity);
        _logger.LogInformation("ProdutoGrupo criado: Id={Id}, Nome={Nome}", created.Id, created.Nome);
        return created;
    }

    public async Task UpdateAsync(ProdutoGrupo entity)
    {
        await _validator.ValidateAndThrowAsync(entity);
        await _repository.UpdateAsync(entity);
        _logger.LogInformation("ProdutoGrupo atualizado: Id={Id}, Nome={Nome}", entity.Id, entity.Nome);
    }

    public async Task DeleteAsync(int id)
    {
        if (!await _repository.ExistsAsync(id))
        {
            _logger.LogWarning("Tentativa de excluir ProdutoGrupo inexistente: Id={Id}", id);
        }
        await _repository.DeleteAsync(id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _repository.ExistsAsync(id);
    }
}
