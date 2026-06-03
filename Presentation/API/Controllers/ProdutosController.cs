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
[Authorize(Policy = Policies.RequireFarmaceutico)]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoService _service;
    private readonly IProdutoControladoService _controladoService;
    private readonly MiniPDVContext _context;

    public ProdutosController(IProdutoService service, IProdutoControladoService controladoService, MiniPDVContext context)
    {
        _service = service;
        _controladoService = controladoService;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _service.GetAllAsync();
        return Ok(items.Adapt<List<ProdutoResponse>>());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item is null) return NotFound();
        return Ok(item.Adapt<ProdutoResponse>());
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProdutoRequest request)
    {
        var entity = new Produto
        {
            Descricao = request.Descricao,
            Ativo = request.Ativo,
            CodBarra = request.CodBarra,
            Controlado = request.Controlado,
            Dosagem = request.Dosagem,
            RegistroMS = request.RegistroMS,
            Preco = request.Preco,
            ProdutoGrupoId = request.ProdutoGrupoId,
            FabricanteId = request.FabricanteId,
            PrincipioAtivoId = request.PrincipioAtivoId,
            Grupo = null!,
            PrincipioAtivo = null!,
            CodBarras = [],
            Estoques = []
        };

        var created = await _service.AddAsync(entity);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.Adapt<ProdutoResponse>());
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProdutoRequest request)
    {
        if (id != request.Id) return BadRequest();

        var isAdmin = User.FindFirst("tipo")?.Value == "Administrador";

        var entity = new Produto
        {
            Id = request.Id,
            Descricao = request.Descricao,
            Ativo = request.Ativo,
            CodBarra = request.CodBarra,
            Controlado = request.Controlado,
            Dosagem = request.Dosagem,
            RegistroMS = request.RegistroMS,
            Preco = isAdmin ? request.Preco : (await _service.GetByIdAsync(id))!.Preco,
            ProdutoGrupoId = request.ProdutoGrupoId,
            FabricanteId = request.FabricanteId,
            PrincipioAtivoId = request.PrincipioAtivoId,
            Grupo = null!,
            PrincipioAtivo = null!,
            CodBarras = [],
            Estoques = []
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

    [HttpPost("controlado")]
    public async Task<IActionResult> CreateControlado([FromBody] CreateProdutoControladoRequest request)
    {
        var entity = new ProdutoControlado
        {
            Descricao = request.Descricao,
            Ativo = request.Ativo,
            CodBarra = request.CodBarra,
            Controlado = true,
            Dosagem = request.Dosagem,
            RegistroMS = request.RegistroMS,
            Preco = request.Preco,
            ProdutoGrupoId = request.ProdutoGrupoId,
            FabricanteId = request.FabricanteId,
            PrincipioAtivoId = request.PrincipioAtivoId,
            Grupo = null!,
            PrincipioAtivo = null!,
            CodBarras = [],
            Estoques = [],
            ClasseTerapeutica = request.ClasseTerapeutica,
            Lista = request.Lista
        };

        var created = await _controladoService.AddAsync(entity);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.Adapt<ProdutoResponse>());
    }

    [HttpPut("controlado/{id}")]
    public async Task<IActionResult> UpdateControlado(int id, [FromBody] UpdateProdutoControladoRequest request)
    {
        if (id != request.Id) return BadRequest();

        var isAdmin = User.FindFirst("tipo")?.Value == "Administrador";

        var entity = new ProdutoControlado
        {
            Id = request.Id,
            Descricao = request.Descricao,
            Ativo = request.Ativo,
            CodBarra = request.CodBarra,
            Controlado = true,
            Dosagem = request.Dosagem,
            RegistroMS = request.RegistroMS,
            Preco = isAdmin ? request.Preco : (await _service.GetByIdAsync(id))!.Preco,
            ProdutoGrupoId = request.ProdutoGrupoId,
            FabricanteId = request.FabricanteId,
            PrincipioAtivoId = request.PrincipioAtivoId,
            Grupo = null!,
            PrincipioAtivo = null!,
            CodBarras = [],
            Estoques = [],
            ClasseTerapeutica = request.ClasseTerapeutica,
            Lista = request.Lista
        };

        await _controladoService.UpdateAsync(entity);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
