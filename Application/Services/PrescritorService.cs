using FluentValidation;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;

namespace minipdv.Application.Services;

public class PrescritorService : IPrescritorService
{
    private readonly IPrescritorRepository _repository;
    private readonly IValidator<Prescritor> _validator;

    public PrescritorService(IPrescritorRepository repository, IValidator<Prescritor> validator)
    {
        _repository = repository;
        _validator = validator;
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
        return await _repository.AddAsync(entity);
    }

    public async Task UpdateAsync(Prescritor entity)
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
