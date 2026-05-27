using FluentValidation;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;

namespace minipdv.Application.Services;

public class FabricanteService : IFabricanteService
{
    private readonly IFabricanteRepository _repository;
    private readonly IValidator<Fabricante> _validator;

    public FabricanteService(IFabricanteRepository repository, IValidator<Fabricante> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<IEnumerable<Fabricante>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Fabricante?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Fabricante?> GetByCnpjAsync(string cnpj)
    {
        return await _repository.GetByCnpjAsync(cnpj);
    }

    public async Task<Fabricante> AddAsync(Fabricante entity)
    {
        await _validator.ValidateAndThrowAsync(entity);
        return await _repository.AddAsync(entity);
    }

    public async Task UpdateAsync(Fabricante entity)
    {
        await _validator.ValidateAndThrowAsync(entity);
        await _repository.UpdateAsync(entity);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _repository.ExistsAsync(id);
    }
}
