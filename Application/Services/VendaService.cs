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

    public async Task<Venda> AddAsync(Venda entity)
    {
        await _validator.ValidateAndThrowAsync(entity);

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var hasControlado = false;

            foreach (var vpe in entity.VendaProdutoEstoques)
            {
                var estoque = await _produtoEstoqueRepository.GetByIdAsync(vpe.ProdutoId, vpe.Lote)
                    ?? throw new InvalidOperationException(
                        $"Estoque não encontrado para o produto {vpe.ProdutoId} lote {vpe.Lote}");

                if (estoque.Quantidade < vpe.Quantidade)
                    throw new InvalidOperationException(
                        $"Estoque insuficiente para o produto {vpe.ProdutoId} lote {vpe.Lote}. " +
                        $"Disponível: {estoque.Quantidade}, solicitado: {vpe.Quantidade}");

                var produto = await _context.Set<Produto>()
                    .FirstAsync(p => p.Id == vpe.ProdutoId);

                if (produto.Controlado)
                {
                    hasControlado = true;

                    if (string.IsNullOrWhiteSpace(estoque.Lote))
                        throw new InvalidOperationException(
                            $"Lote é obrigatório para o produto controlado {vpe.ProdutoId}");

                    if (string.IsNullOrWhiteSpace(produto.RegistroMS))
                        throw new InvalidOperationException(
                            $"Registro MS é obrigatório para o produto controlado {vpe.ProdutoId}");
                }

                estoque.Quantidade -= vpe.Quantidade;
                vpe.ProdutoEstoque = estoque;
            }

            if (hasControlado && entity.ReceitaId is null or 0)
                throw new InvalidOperationException(
                    "Receita é obrigatória quando a venda contém produtos controlados");

            entity.CriadoEm = DateTime.UtcNow;
            _context.Vendas.Add(entity);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return entity;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task DeleteAsync(int id)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var entity = await _context.Vendas
                .Include(v => v.VendaProdutoEstoques)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (entity is null) return;

            foreach (var vpe in entity.VendaProdutoEstoques)
            {
                var estoque = await _produtoEstoqueRepository.GetByIdAsync(vpe.ProdutoId, vpe.Lote);
                if (estoque is not null)
                    estoque.Quantidade += vpe.Quantidade;
            }

            _context.Vendas.Remove(entity);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
