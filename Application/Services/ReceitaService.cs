using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;
using minipdv.Infrastructure.Data.Context;

namespace minipdv.Application.Services;

public class ReceitaService : IReceitaService
{
    private readonly IReceitaRepository _repository;
    private readonly IProdutoEstoqueRepository _produtoEstoqueRepository;
    private readonly MiniPDVContext _context;
    private readonly IValidator<Receita> _validator;
    private readonly ILogger<ReceitaService> _logger;

    public ReceitaService(
        IReceitaRepository repository,
        IProdutoEstoqueRepository produtoEstoqueRepository,
        MiniPDVContext context,
        IValidator<Receita> validator,
        ILogger<ReceitaService> logger)
    {
        _repository = repository;
        _produtoEstoqueRepository = produtoEstoqueRepository;
        _context = context;
        _validator = validator;
        _logger = logger;
    }

    public async Task<IEnumerable<Receita>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Receita?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Receita> AddAsync(Receita entity)
    {
        await _validator.ValidateAndThrowAsync(entity);

        foreach (var rpe in entity.ReceitaProdutoEstoques)
        {
            var estoque = await _produtoEstoqueRepository.GetByIdAsync(rpe.ProdutoId, rpe.Lote)
                ?? throw new InvalidOperationException(
                    $"Estoque não encontrado para o produto {rpe.ProdutoId} lote {rpe.Lote}");

            if (estoque.Quantidade < rpe.Quantidade)
            {
                _logger.LogWarning("Estoque insuficiente para receita: ProdutoId={ProdutoId}, Lote={Lote}, Disponivel={Disponivel}, Solicitado={Solicitado}",
                    rpe.ProdutoId, rpe.Lote, estoque.Quantidade, rpe.Quantidade);
                throw new InvalidOperationException(
                    $"Estoque insuficiente para o produto {rpe.ProdutoId} lote {rpe.Lote}. " +
                    $"Disponível: {estoque.Quantidade}, solicitado: {rpe.Quantidade}");
            }

            estoque.Quantidade -= rpe.Quantidade;
            rpe.ProdutoEstoque = estoque;
        }

        entity.CriadoEm = DateTime.UtcNow;
        _context.Receitas.Add(entity);

        _logger.LogInformation("Receita adicionada ao change tracker: PrescritorId={PrescritorId}, PacienteId={PacienteId}, Itens={Itens}",
            entity.PrescritorId, entity.PacienteId, entity.ReceitaProdutoEstoques?.Count ?? 0);
        return entity;
    }

    public async Task UpdateAsync(Receita entity)
    {
        await _validator.ValidateAndThrowAsync(entity);

        var existing = await _context.Receitas
            .Include(r => r.ReceitaProdutoEstoques)
            .FirstAsync(r => r.Id == entity.Id);

        var oldItems = existing.ReceitaProdutoEstoques.ToList();
        var newItems = entity.ReceitaProdutoEstoques.ToList();
        var affectedProdutos = new HashSet<int>();

        foreach (var old in oldItems)
        {
            var stillInNew = newItems.Any(n => n.ProdutoId == old.ProdutoId && n.Lote == old.Lote);
            if (!stillInNew)
            {
                var estoque = await _produtoEstoqueRepository.GetByIdAsync(old.ProdutoId, old.Lote);
                if (estoque is not null)
                    estoque.Quantidade += old.Quantidade;
                affectedProdutos.Add(old.ProdutoId);
            }
        }

        foreach (var newItem in newItems)
        {
            var wasInOld = oldItems.Any(o => o.ProdutoId == newItem.ProdutoId && o.Lote == newItem.Lote);
            if (!wasInOld)
            {
                var estoque = await _produtoEstoqueRepository.GetByIdAsync(newItem.ProdutoId, newItem.Lote)
                    ?? throw new InvalidOperationException(
                        $"Estoque não encontrado para o produto {newItem.ProdutoId} lote {newItem.Lote}");

                if (estoque.Quantidade < newItem.Quantidade)
                {
                    _logger.LogWarning("Estoque insuficiente ao atualizar receita: ProdutoId={ProdutoId}, Lote={Lote}, Disponivel={Disponivel}, Solicitado={Solicitado}",
                        newItem.ProdutoId, newItem.Lote, estoque.Quantidade, newItem.Quantidade);
                    throw new InvalidOperationException(
                        $"Estoque insuficiente para o produto {newItem.ProdutoId} lote {newItem.Lote}. " +
                        $"Disponível: {estoque.Quantidade}, solicitado: {newItem.Quantidade}");
                }

                estoque.Quantidade -= newItem.Quantidade;
                newItem.ProdutoEstoque = estoque;
                affectedProdutos.Add(newItem.ProdutoId);
            }
        }

        existing.DataReceita = entity.DataReceita;
        existing.DataCadastro = entity.DataCadastro;
        existing.PrescritorId = entity.PrescritorId;
        existing.PacienteId = entity.PacienteId;
        existing.CompradorId = entity.CompradorId;

        _context.Set<ReceitaProdutoEstoque>().RemoveRange(oldItems);
        existing.ReceitaProdutoEstoques = newItems;
        existing.AtualizadoEm = DateTime.UtcNow;

        _logger.LogInformation("Receita marcada para atualização: ReceitaId={ReceitaId}, Itens={Itens}", entity.Id, newItems.Count);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Receitas
            .Include(r => r.ReceitaProdutoEstoques)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (entity is null)
        {
            _logger.LogWarning("Tentativa de excluir receita inexistente: ReceitaId={Id}", id);
            return;
        }

        if (entity.VendaId.HasValue)
        {
            _logger.LogWarning("Tentativa de excluir receita vinculada a venda: ReceitaId={Id}, VendaId={VendaId}", id, entity.VendaId);
            throw new InvalidOperationException(
                "Não é possível excluir uma receita vinculada a uma venda. Cancele a venda primeiro.");
        }

        foreach (var rpe in entity.ReceitaProdutoEstoques)
        {
            var estoque = await _produtoEstoqueRepository.GetByIdAsync(rpe.ProdutoId, rpe.Lote);
            if (estoque is not null)
                estoque.Quantidade += rpe.Quantidade;
        }

        _context.Receitas.Remove(entity);

        _logger.LogInformation("Receita marcada para exclusão: ReceitaId={Id}, PrescritorId={PrescritorId}", id, entity.PrescritorId);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _repository.ExistsAsync(id);
    }
}
