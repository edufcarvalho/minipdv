using FluentValidation;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;

namespace minipdv.Application.Services;

public class ProdutoCodBarraService : IProdutoCodBarraService
{
    private readonly IProdutoCodBarraRepository _repository;
    private readonly IValidator<ProdutoCodBarra> _validator;

    public ProdutoCodBarraService(IProdutoCodBarraRepository repository, IValidator<ProdutoCodBarra> validator)
    {
        _repository = repository;
        _validator = validator;
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
        return await _repository.AddAsync(entity);
    }

    public async Task UpdateAsync(ProdutoCodBarra entity)
    {
        await _validator.ValidateAndThrowAsync(entity);
        await _repository.UpdateAsync(entity);
    }

    public async Task DeleteAsync(int codBarra)
    {
        await _repository.DeleteAsync(codBarra);
    }

    public async Task<bool> ExistsAsync(int codBarra)
    {
        return await _repository.ExistsAsync(codBarra);
    }
}
