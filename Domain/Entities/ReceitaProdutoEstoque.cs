using System.ComponentModel.DataAnnotations.Schema;

namespace minipdv.Domain.Entities;

public class ReceitaProdutoEstoque
{
    public required int ReceitaId { get; set; }
    [ForeignKey(nameof(ReceitaId))]
    public required virtual Receita Receita { get; set; }
    public required int ProdutoId { get; set; }
    public required string Lote { get; set; }
    public required int Quantidade { get; set; } = 1;
    public required virtual ProdutoEstoque ProdutoEstoque { get; set; }
}
