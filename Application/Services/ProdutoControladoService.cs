using FluentValidation;
using Microsoft.Extensions.Logging;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;

namespace minipdv.Application.Services;

public class ProdutoControladoService : IProdutoControladoService
{
    private readonly IProdutoControladoRepository _repository;
    private readonly IValidator<ProdutoControlado> _validator;
    private readonly ILogger<ProdutoControladoService> _logger;

    public ProdutoControladoService(IProdutoControladoRepository repository, IValidator<ProdutoControlado> validator, ILogger<ProdutoControladoService> logger)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<IEnumerable<ProdutoControlado>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<ProdutoControlado?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<ProdutoControlado?> GetByRegistroMsAsync(string registroMs)
    {
        return await _repository.GetByRegistroMsAsync(registroMs);
    }

    public async Task<ProdutoControlado> AddAsync(ProdutoControlado entity)
    {
        await _validator.ValidateAndThrowAsync(entity);
        var created = await _repository.AddAsync(entity);
        _logger.LogInformation("ProdutoControlado criado: Id={Id}, RegistroMS={RegistroMS}", created.Id, created.RegistroMS);
        return created;
    }

    public async Task UpdateAsync(ProdutoControlado entity)
    {
        await _validator.ValidateAndThrowAsync(entity);
        await _repository.UpdateAsync(entity);
        _logger.LogInformation("ProdutoControlado atualizado: Id={Id}", entity.Id);
    }

    public async Task DeleteAsync(int id)
    {
        if (!await _repository.ExistsAsync(id))
        {
            _logger.LogWarning("Tentativa de excluir ProdutoControlado inexistente: Id={Id}", id);
        }
        await _repository.DeleteAsync(id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _repository.ExistsAsync(id);
    }
}
