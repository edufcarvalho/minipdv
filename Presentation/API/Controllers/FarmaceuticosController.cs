using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using minipdv.Application.DTOs;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Infrastructure.Data.Context;
using minipdv.Infrastructure.Security;

namespace minipdv.Presentation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = Policies.RequireFarmaceutico)]
public class FarmaceuticosController : ControllerBase
{
    private readonly IFarmaceuticoService _service;
    private readonly MiniPDVContext _context;

    public FarmaceuticosController(IFarmaceuticoService service, MiniPDVContext context)
    {
        _service = service;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _service.GetAllAsync();
        return Ok(items.Adapt<List<FarmaceuticoResponse>>());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item is null) return NotFound();
        return Ok(item.Adapt<FarmaceuticoResponse>());
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateFarmaceuticoRequest request)
    {
        var existing = await _service.GetByIdAsync(id);
        if (existing is null) return NotFound();

        existing.Nome = request.Nome;
        existing.Login = request.Login;
        existing.Ativo = request.Ativo;
        existing.Crf = request.Crf;
        existing.ContatoId = request.ContatoId;

        if (!string.IsNullOrEmpty(request.Password))
            existing.PasswordHash = PasswordHasher.Hash(request.Password);

        await _service.UpdateAsync(existing);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
