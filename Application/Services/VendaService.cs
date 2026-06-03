using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Infrastructure.Data.Context;

namespace minipdv.Application.Services;

public class VendaService : IVendaService
{
    private readonly MiniPDVContext _context;
    private readonly IValidator<Venda> _validator;
    private readonly ILogger<VendaService> _logger;

    public VendaService(
        MiniPDVContext context,
        IValidator<Venda> validator,
        ILogger<VendaService> logger)
    {
        _context = context;
        _validator = validator;
        _logger = logger;
    }

    public async Task<IEnumerable<Venda>> GetAllAsync()
    {
        return await _context.Vendas
            .Include(v => v.Vendedor)
            .Include(v => v.Cliente)
            .Include(v => v.VendaItens).ThenInclude(vi => vi.Produto)
            .ToListAsync();
    }

    public async Task<Venda?> GetByIdAsync(int id)
    {
        return await _context.Vendas
            .Include(v => v.Vendedor)
            .Include(v => v.Cliente)
            .Include(v => v.VendaItens).ThenInclude(vi => vi.Produto)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<Venda> AddAsync(Venda entity, List<int>? receitaIds = null)
    {
        await _validator.ValidateAndThrowAsync(entity);

        foreach (var item in entity.VendaItens)
        {
            var produto = await _context.Set<Produto>()
                .FirstAsync(p => p.Id == item.ProdutoId);

            if (produto.Estoque < item.Quantidade)
            {
                _logger.LogWarning("Estoque insuficiente para venda: ProdutoId={ProdutoId}, Produto={Descricao}, Estoque={Estoque}, Solicitado={Solicitado}",
                    produto.Id, produto.Descricao, produto.Estoque, item.Quantidade);
                throw new InvalidOperationException(
                    $"Estoque insuficiente para o produto {produto.Descricao}. " +
                    $"Disponível: {produto.Estoque}, solicitado: {item.Quantidade}");
            }

            item.PrecoUnitario = produto.Preco;
            produto.Estoque -= item.Quantidade;
        }

        entity.Total = entity.VendaItens.Sum(i => i.PrecoUnitario * i.Quantidade);

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

        var totalItens = entity.VendaItens?.Count ?? 0;
        _logger.LogInformation("Venda adicionada ao change tracker: VendedorId={VendedorId}, TotalItens={TotalItens}, ReceitasVinculadas={Receitas}",
            entity.VendedorId, totalItens, receitaIds?.Count ?? 0);
        return entity;
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Vendas
            .Include(v => v.VendaItens)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (entity is null)
        {
            _logger.LogWarning("Tentativa de cancelar venda inexistente: VendaId={Id}", id);
            return;
        }

        foreach (var item in entity.VendaItens)
        {
            var produto = await _context.Set<Produto>()
                .FirstAsync(p => p.Id == item.ProdutoId);

            produto.Estoque += item.Quantidade;
        }

        entity.CanceladoEm = DateTime.UtcNow;
        entity.AtualizadoEm = DateTime.UtcNow;

        _logger.LogInformation("Venda marcada para cancelamento: VendaId={VendaId}, VendedorId={VendedorId}", entity.Id, entity.VendedorId);
    }
}
