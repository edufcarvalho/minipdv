using FluentValidation;
using Microsoft.EntityFrameworkCore;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;
using minipdv.Infrastructure.Data.Context;

namespace minipdv.Application.Services;

public class VendaService : IVendaService
{
    private readonly IVendaRepository _repository;
    private readonly IProdutoEstoqueRepository _produtoEstoqueRepository;
    private readonly MiniPDVContext _context;
    private readonly IValidator<Venda> _validator;

    public VendaService(
        IVendaRepository repository,
        IProdutoEstoqueRepository produtoEstoqueRepository,
        MiniPDVContext context,
        IValidator<Venda> validator)
    {
        _repository = repository;
        _produtoEstoqueRepository = produtoEstoqueRepository;
        _context = context;
        _validator = validator;
    }

    public async Task<IEnumerable<Venda>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Venda?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Venda> AddAsync(Venda entity, List<int>? receitaIds = null)
    {
        await _validator.ValidateAndThrowAsync(entity);

        foreach (var item in entity.VendaItens)
        {
            var produto = await _context.Set<Produto>()
                .FirstAsync(p => p.Id == item.ProdutoId);

            produto.Estoque -= item.Quantidade;
        }

        entity.CriadoEm = DateTime.UtcNow;
        _context.Vendas.Add(entity);

        if (receitaIds is { Count: > 0 })
        {
            var receitas = await _context.Receitas
                .Where(r => receitaIds.Contains(r.Id))
                .ToListAsync();

            foreach (var receita in receitas)
            {
                receita.Venda = entity;
                receita.AtualizadoEm = DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Vendas
            .Include(v => v.VendaItens)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (entity is null) return;

        foreach (var item in entity.VendaItens)
        {
            var produto = await _context.Set<Produto>()
                .FirstAsync(p => p.Id == item.ProdutoId);

            produto.Estoque += item.Quantidade;
        }

        entity.CanceladoEm = DateTime.UtcNow;
        entity.AtualizadoEm = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }
}
