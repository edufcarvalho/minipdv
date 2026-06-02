using FluentValidation;
using Microsoft.Extensions.Logging;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;

namespace minipdv.Application.Services;

public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _repository;
    private readonly IValidator<Produto> _validator;
    private readonly ILogger<ProdutoService> _logger;

    public ProdutoService(IProdutoRepository repository, IValidator<Produto> validator, ILogger<ProdutoService> logger)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<IEnumerable<Produto>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Produto?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Produto?> GetByCodBarraAsync(int codBarra)
    {
        return await _repository.GetByCodBarraAsync(codBarra);
    }

    public async Task<Produto> AddAsync(Produto entity)
    {
        await _validator.ValidateAndThrowAsync(entity);
        var created = await _repository.AddAsync(entity);
        _logger.LogInformation("Produto criado: Id={Id}, Descricao={Descricao}, CodBarra={CodBarra}", created.Id, created.Descricao, created.CodBarra);
        return created;
    }

    public async Task UpdateAsync(Produto entity)
    {
        await _validator.ValidateAndThrowAsync(entity);
        await _repository.UpdateAsync(entity);
        _logger.LogInformation("Produto atualizado: Id={Id}, Descricao={Descricao}", entity.Id, entity.Descricao);
    }

    public async Task DeleteAsync(int id)
    {
        if (!await _repository.ExistsAsync(id))
        {
            _logger.LogWarning("Tentativa de excluir produto inexistente: Id={Id}", id);
        }
        await _repository.DeleteAsync(id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _repository.ExistsAsync(id);
    }
}
