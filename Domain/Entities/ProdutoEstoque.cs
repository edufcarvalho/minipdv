using System.ComponentModel.DataAnnotations.Schema;

namespace minipdv.Domain.Entities;

public class ProdutoEstoque
{
    public required int ProdutoId { get; set; }
    [ForeignKey(nameof(ProdutoId))]
    public required virtual Produto Produto { get; set; }
    public string? Lote { get; set; }
    public DateTime? Fabricacao { get; set; }
    public string? RegistroMS { get; set; }
    public DateTime? Validade { get; set; }
    public required int Quantidade { get; set; }
    public virtual ICollection<ReceitaProdutoEstoque> ReceitaProdutoEstoques { get; set; } = [];
    public virtual ICollection<VendaProdutoEstoque> VendaProdutoEstoques { get; set; } = [];
    public string? ProdutoDescricao => Produto?.Descricao;
    [NotMapped]
    public IEnumerable<Receita> Receitas =>
        ReceitaProdutoEstoques?.Select(rpe => rpe.Receita).Where(r => r is not null)!;
}
