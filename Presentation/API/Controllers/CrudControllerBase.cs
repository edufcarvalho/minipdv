using Microsoft.AspNetCore.Mvc;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities.Base;
using minipdv.Infrastructure.Data.Context;

namespace minipdv.Presentation.API.Controllers;

public abstract class CrudControllerBase<TEntity, TService> : ControllerBase
    where TEntity : Entity
    where TService : ICrudService<TEntity>
{
    protected readonly TService Service;
    protected readonly MiniPDVContext Context;

    protected CrudControllerBase(TService service, MiniPDVContext context)
    {
        Service = service;
        Context = context;
    }

    [HttpGet]
    public virtual async Task<IActionResult> GetAll()
    {
        var items = await Service.GetAllAsync();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public virtual async Task<IActionResult> GetById(int id)
    {
        var item = await Service.GetByIdAsync(id);
        if (item is null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public virtual async Task<IActionResult> Create([FromBody] TEntity entity)
    {
        var created = await Service.AddAsync(entity);
        await Context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public virtual async Task<IActionResult> Update(int id, [FromBody] TEntity entity)
    {
        if (id != entity.Id) return BadRequest();
        await Service.UpdateAsync(entity);
        await Context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public virtual async Task<IActionResult> Delete(int id)
    {
        await Service.DeleteAsync(id);
        await Context.SaveChangesAsync();
        return NoContent();
    }
}
