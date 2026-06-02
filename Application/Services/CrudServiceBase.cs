using FluentValidation;
using Microsoft.Extensions.Logging;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities.Base;
using minipdv.Domain.Interfaces;

namespace minipdv.Application.Services;

public abstract class CrudServiceBase<TEntity, TRepository> : ICrudService<TEntity>
    where TEntity : Entity
    where TRepository : class, IRepository<TEntity>
{
    protected readonly TRepository Repository;
    protected readonly IValidator<TEntity> Validator;
    protected readonly ILogger Logger;

    protected CrudServiceBase(TRepository repository, IValidator<TEntity> validator, ILogger logger)
    {
        Repository = repository;
        Validator = validator;
        Logger = logger;
    }

    protected virtual string EntityDisplayName => typeof(TEntity).Name;

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await Repository.GetAllAsync();
    }

    public virtual async Task<TEntity?> GetByIdAsync(int id)
    {
        return await Repository.GetByIdAsync(id);
    }

    public virtual async Task<TEntity> AddAsync(TEntity entity)
    {
        await Validator.ValidateAndThrowAsync(entity);
        var created = await Repository.AddAsync(entity);
        Logger.LogInformation("{Entity} criado: Id={Id}", EntityDisplayName, created.Id);
        return created;
    }

    public virtual async Task UpdateAsync(TEntity entity)
    {
        await Validator.ValidateAndThrowAsync(entity);
        await Repository.UpdateAsync(entity);
        Logger.LogInformation("{Entity} atualizado: Id={Id}", EntityDisplayName, entity.Id);
    }

    public virtual async Task DeleteAsync(int id)
    {
        if (!await Repository.ExistsAsync(id))
        {
            Logger.LogWarning("Tentativa de excluir {Entity} inexistente: Id={Id}", EntityDisplayName, id);
            return;
        }
        await Repository.DeleteAsync(id);
        Logger.LogInformation("{Entity} removido: Id={Id}", EntityDisplayName, id);
    }

    public virtual async Task<bool> ExistsAsync(int id)
    {
        return await Repository.ExistsAsync(id);
    }
}
