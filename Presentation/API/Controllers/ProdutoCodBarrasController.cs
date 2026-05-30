using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using minipdv.Application.DTOs;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;

namespace minipdv.Presentation.API.Controllers;

[ApiController]
[Route("api/produtos/{produtoId}/codbarras")]
[Authorize(Policy = "RequireFarmaceutico")]
public class ProdutoCodBarrasController : ControllerBase
{
    private readonly IProdutoCodBarraService _service;

    public ProdutoCodBarrasController(IProdutoCodBarraService service)
    {
        _service = service;
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

        try
        {
            var created = await _service.AddAsync(entity);
            return CreatedAtAction(nameof(GetById), new { produtoId, codBarra = created.CodBarra }, created);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { errors = ex.Errors });
        }
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

        try
        {
            await _service.UpdateAsync(entity);
            return NoContent();
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { errors = ex.Errors });
        }
    }

    [HttpDelete("{codBarra}")]
    public async Task<IActionResult> Delete(int codBarra)
    {
        await _service.DeleteAsync(codBarra);
        return NoContent();
    }
}
