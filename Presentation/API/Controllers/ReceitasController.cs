using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using minipdv.Application.DTOs;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;

namespace minipdv.Presentation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "RequireFarmaceutico")]
public class ReceitasController : ControllerBase
{
    private readonly IReceitaService _service;

    public ReceitasController(IReceitaService service)
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

        try
        {
            var created = await _service.AddAsync(entity);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { errors = ex.Errors });
        }
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

        try
        {
            await _service.UpdateAsync(entity);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { errors = ex.Errors });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
