using FluentValidation;
using Microsoft.Extensions.Logging;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;

namespace minipdv.Application.Services;

public class ProdutoCodBarraService : IProdutoCodBarraService
{
    private readonly IProdutoCodBarraRepository _repository;
    private readonly IValidator<ProdutoCodBarra> _validator;
    private readonly ILogger<ProdutoCodBarraService> _logger;

    public ProdutoCodBarraService(IProdutoCodBarraRepository repository, IValidator<ProdutoCodBarra> validator, ILogger<ProdutoCodBarraService> logger)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<IEnumerable<ProdutoCodBarra>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<IEnumerable<ProdutoCodBarra>> GetByProdutoIdAsync(int produtoId)
    {
        return await _repository.GetByProdutoIdAsync(produtoId);
    }

    public async Task<ProdutoCodBarra?> GetByCodBarraAsync(int codBarra)
    {
        return await _repository.GetByCodBarraAsync(codBarra);
    }

    public async Task<ProdutoCodBarra> AddAsync(ProdutoCodBarra entity)
    {
        await _validator.ValidateAndThrowAsync(entity);
        var created = await _repository.AddAsync(entity);
        _logger.LogInformation("ProdutoCodBarra criado: CodBarra={CodBarra}, ProdutoId={ProdutoId}", created.CodBarra, created.ProdutoId);
        return created;
    }

    public async Task UpdateAsync(ProdutoCodBarra entity)
    {
        await _validator.ValidateAndThrowAsync(entity);
        await _repository.UpdateAsync(entity);
        _logger.LogInformation("ProdutoCodBarra atualizado: CodBarra={CodBarra}", entity.CodBarra);
    }

    public async Task DeleteAsync(int codBarra)
    {
        if (!await _repository.ExistsAsync(codBarra))
        {
            _logger.LogWarning("Tentativa de excluir ProdutoCodBarra inexistente: CodBarra={CodBarra}", codBarra);
            return;
        }
        await _repository.DeleteAsync(codBarra);
        _logger.LogInformation("ProdutoCodBarra removido: CodBarra={CodBarra}", codBarra);
    }

    public async Task<bool> ExistsAsync(int codBarra)
    {
        return await _repository.ExistsAsync(codBarra);
    }
}
