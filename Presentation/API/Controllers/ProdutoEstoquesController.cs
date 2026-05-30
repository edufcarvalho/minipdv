using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using minipdv.Application.DTOs;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;

namespace minipdv.Presentation.API.Controllers;

[ApiController]
[Route("api/produtos/{produtoId}/estoques")]
[Authorize(Policy = "RequireFarmaceutico")]
public class ProdutoEstoquesController : ControllerBase
{
    private readonly IProdutoEstoqueService _service;

    public ProdutoEstoquesController(IProdutoEstoqueService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(int produtoId)
    {
        var items = await _service.GetByProdutoIdAsync(produtoId);
        return Ok(items);
    }

    [HttpGet("{lote}")]
    public async Task<IActionResult> GetById(int produtoId, string lote)
    {
        var item = await _service.GetByIdAsync(produtoId, lote);
        if (item is null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create(int produtoId, [FromBody] CreateProdutoEstoqueRequest request)
    {
        if (produtoId != request.ProdutoId) return BadRequest();

        var entity = new ProdutoEstoque
        {
            ProdutoId = request.ProdutoId,
            Lote = request.Lote,
            Fabricacao = request.Fabricacao,
            Validade = request.Validade,
            Quantidade = request.Quantidade,
            Produto = null!
        };

        try
        {
            var created = await _service.AddAsync(entity);
            return CreatedAtAction(nameof(GetById), new { produtoId, lote = created.Lote }, created);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { errors = ex.Errors });
        }
    }

    [HttpPut("{lote}")]
    public async Task<IActionResult> Update(int produtoId, string lote, [FromBody] UpdateProdutoEstoqueRequest request)
    {
        if (produtoId != request.ProdutoId || lote != request.Lote) return BadRequest();

        var entity = new ProdutoEstoque
        {
            ProdutoId = request.ProdutoId,
            Lote = request.Lote,
            Fabricacao = request.Fabricacao,
            Validade = request.Validade,
            Quantidade = request.Quantidade,
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

    [HttpDelete("{lote}")]
    public async Task<IActionResult> Delete(int produtoId, string lote)
    {
        await _service.DeleteAsync(produtoId, lote);
        return NoContent();
    }
}
