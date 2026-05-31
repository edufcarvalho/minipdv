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
    private readonly IProdutoService _produtoService;

    public ProdutoEstoquesController(IProdutoEstoqueService service, IProdutoService produtoService)
    {
        _service = service;
        _produtoService = produtoService;
    }

    [HttpGet("~/api/estoques")]
    public async Task<IActionResult> GetAll()
    {
        var items = await _service.GetAllAsync();
        return Ok(items);
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

        var produto = await _produtoService.GetByIdAsync(request.ProdutoId);

        if (produto is null)
            return BadRequest(new { error = "Produto não encontrado" });

        var entity = new ProdutoEstoque
        {
            ProdutoId = request.ProdutoId,
            Lote = request.Lote,
            Fabricacao = request.Fabricacao,
            Validade = request.Validade,
            Quantidade = request.Quantidade,
            RegistroMS = request.RegistroMS,
            Produto = produto
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

        var produto = await _produtoService.GetByIdAsync(request.ProdutoId);

        if (produto is null)
            return BadRequest(new { error = "Produto não encontrado" });

        var entity = new ProdutoEstoque
        {
            ProdutoId = request.ProdutoId,
            Lote = request.Lote,
            Fabricacao = request.Fabricacao,
            Validade = request.Validade,
            Quantidade = request.Quantidade,
            RegistroMS = request.RegistroMS,
            Produto = produto
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
