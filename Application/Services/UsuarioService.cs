using FluentValidation;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;

namespace minipdv.Application.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _repository;
    private readonly IValidator<Usuario> _validator;

    public UsuarioService(IUsuarioRepository repository, IValidator<Usuario> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<IEnumerable<Usuario>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Usuario?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Usuario?> GetByLoginAsync(string login)
    {
        return await _repository.GetByLoginAsync(login);
    }

    public async Task<Usuario> AddAsync(Usuario entity)
    {
        await _validator.ValidateAndThrowAsync(entity);
        return await _repository.AddAsync(entity);
    }

    public async Task UpdateAsync(Usuario entity)
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
