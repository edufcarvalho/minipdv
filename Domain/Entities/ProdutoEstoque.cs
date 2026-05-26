using System.ComponentModel.DataAnnotations.Schema;

namespace minipdv.Domain.Entities;

public class ProdutoEstoque
{
    public required int ProdutoId { get; set; }
    [ForeignKey(nameof(ProdutoId))]
    public required virtual Produto Produto { get; set; }
    public required string Lote { get; set; }
    public DateTime? Fabricacao { get; set; }
    public DateTime? Validade { get; set; }
    public required int Quantidade { get; set; }
}
