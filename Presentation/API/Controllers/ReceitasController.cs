using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using minipdv.Application.DTOs;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Infrastructure.Data.Context;

namespace minipdv.Presentation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = Policies.RequireFarmaceutico)]
public class ReceitasController : ControllerBase
{
    private readonly IReceitaService _service;
    private readonly MiniPDVContext _context;

    public ReceitasController(IReceitaService service, MiniPDVContext context)
    {
        _service = service;
        _context = context;
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
    public async Task<IActionResult> Create([FromBody] CreateReceitaRequest request)
    {
        var entity = new Receita
        {
            DataReceita = request.DataReceita ?? DateTime.UtcNow,
            DataCadastro = request.DataCadastro ?? DateTime.UtcNow,
            PrescritorId = request.PrescritorId,
            PacienteId = request.PacienteId,
            CompradorId = request.CompradorId,
            Prescritor = null!,
            Paciente = null!,
            Comprador = null!,
            ReceitaProdutoEstoques = request.Produtos?
                .Select(p => new ReceitaProdutoEstoque
                {
                    ReceitaId = 0,
                    ProdutoId = p.ProdutoId,
                    Lote = p.Lote,
                    Quantidade = p.Quantidade,
                    Receita = null!,
                    ProdutoEstoque = null!
                })
                .ToList() ?? []
        };

        var created = await _service.AddAsync(entity);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateReceitaRequest request)
    {
        if (id != request.Id) return BadRequest();

        var entity = new Receita
        {
            Id = request.Id,
            DataReceita = request.DataReceita ?? DateTime.UtcNow,
            DataCadastro = request.DataCadastro ?? DateTime.UtcNow,
            PrescritorId = request.PrescritorId,
            PacienteId = request.PacienteId,
            CompradorId = request.CompradorId,
            Prescritor = null!,
            Paciente = null!,
            Comprador = null!,
            ReceitaProdutoEstoques = request.Produtos?
                .Select(p => new ReceitaProdutoEstoque
                {
                    ReceitaId = id,
                    ProdutoId = p.ProdutoId,
                    Lote = p.Lote,
                    Quantidade = p.Quantidade,
                    Receita = null!,
                    ProdutoEstoque = null!
                })
                .ToList() ?? []
        };

        await _service.UpdateAsync(entity);
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
