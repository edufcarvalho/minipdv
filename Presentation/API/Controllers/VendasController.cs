using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using minipdv.Application.DTOs;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Infrastructure.Data.Context;

namespace minipdv.Presentation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = Policies.RequireAutenticado)]
public class VendasController : ControllerBase
{
    private readonly IVendaService _service;
    private readonly MiniPDVContext _context;

    public VendasController(IVendaService service, MiniPDVContext context)
    {
        _service = service;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _service.GetAllAsync();
        return Ok(items.Adapt<List<VendaResponse>>());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item is null) return NotFound();
        return Ok(item.Adapt<VendaResponse>());
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateVendaRequest request)
    {
        var entity = new Venda
        {
            VendedorId = request.VendedorId,
            ClienteId = request.ClienteId,
            Vendedor = null!,
            Cliente = null!,
            VendaItens = request.Produtos
                .Select((p, index) => new VendaItem
                {
                    VendaId = 0,
                    ProdutoId = p.ProdutoId,
                    Posicao = index + 1,
                    Quantidade = p.Quantidade,
                    Venda = null!,
                    Produto = null!
                })
                .ToList()
        };

        var created = await _service.AddAsync(entity, request.ReceitaIds);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.Adapt<VendaResponse>());
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
