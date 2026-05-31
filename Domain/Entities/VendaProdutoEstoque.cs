using System.ComponentModel.DataAnnotations.Schema;

namespace minipdv.Domain.Entities;

public class VendaProdutoEstoque
{
    public required int VendaId { get; set; }
    [ForeignKey(nameof(VendaId))]
    public required virtual Venda Venda { get; set; }
    public required int ProdutoId { get; set; }
    public required string Lote { get; set; }
    public required int Quantidade { get; set; } = 1;
    public virtual ProdutoEstoque? ProdutoEstoque { get; set; }
}
