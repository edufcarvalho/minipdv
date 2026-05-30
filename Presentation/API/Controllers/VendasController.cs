using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using minipdv.Application.DTOs;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;

namespace minipdv.Presentation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "RequireAutenticado")]
public class VendasController : ControllerBase
{
    private readonly IVendaService _service;

    public VendasController(IVendaService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _service.GetAllAsync();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item is null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateVendaRequest request)
    {
        var entity = new Venda
            {
                VendedorId = request.VendedorId,
                ClienteId = request.ClienteId,
                ReceitaId = request.ReceitaId,
                Vendedor = null!,
                Cliente = null!,
                VendaProdutoEstoques = request.Produtos
                    .Select(p => new VendaProdutoEstoque
                    {
                        VendaId = 0,
                        ProdutoId = p.ProdutoId,
                        Lote = p.Lote,
                        Quantidade = p.Quantidade,
                        Venda = null!,
                        ProdutoEstoque = null!
                    })
                    .ToList()
            };

        try
        {
            var created = await _service.AddAsync(entity);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (FluentValidation.ValidationException ex)
        {
            return BadRequest(new { errors = ex.Errors });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
