using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using minipdv.Application.DTOs;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Infrastructure.Data.Context;

namespace minipdv.Presentation.API.Controllers;

[ApiController]
[Route("api/produtos/{produtoId}/codbarras")]
[Authorize(Policy = Policies.RequireFarmaceutico)]
public class ProdutoCodBarrasController : ControllerBase
{
    private readonly IProdutoCodBarraService _service;
    private readonly MiniPDVContext _context;

    public ProdutoCodBarrasController(IProdutoCodBarraService service, MiniPDVContext context)
    {
        _service = service;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(int produtoId)
    {
        var items = await _service.GetByProdutoIdAsync(produtoId);
        return Ok(items);
    }

    [HttpGet("{codBarra}")]
    public async Task<IActionResult> GetById(int codBarra)
    {
        var item = await _service.GetByCodBarraAsync(codBarra);
        if (item is null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create(int produtoId, [FromBody] CreateProdutoCodBarraRequest request)
    {
        if (produtoId != request.ProdutoId) return BadRequest();

        var entity = new ProdutoCodBarra
        {
            CodBarra = request.CodBarra,
            ProdutoId = request.ProdutoId,
            Produto = null!
        };

        var created = await _service.AddAsync(entity);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { produtoId, codBarra = created.CodBarra }, created);
    }

    [HttpPut("{codBarra}")]
    public async Task<IActionResult> Update(int codBarra, [FromBody] CreateProdutoCodBarraRequest request)
    {
        if (codBarra != request.CodBarra) return BadRequest();

        var entity = new ProdutoCodBarra
        {
            CodBarra = request.CodBarra,
            ProdutoId = request.ProdutoId,
            Produto = null!
        };

        await _service.UpdateAsync(entity);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{codBarra}")]
    public async Task<IActionResult> Delete(int codBarra)
    {
        await _service.DeleteAsync(codBarra);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
