using FluentValidation;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;

namespace minipdv.Application.Services;

public class FarmaceuticoService : IFarmaceuticoService
{
    private readonly IFarmaceuticoRepository _repository;
    private readonly IValidator<Farmaceutico> _validator;

    public FarmaceuticoService(IFarmaceuticoRepository repository, IValidator<Farmaceutico> validator)
    {
        _repository = repository;
        _validator = validator;
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
        return await _repository.AddAsync(entity);
    }

    public async Task UpdateAsync(Farmaceutico entity)
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
