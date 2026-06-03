using System.ComponentModel.DataAnnotations.Schema;

namespace minipdv.Domain.Entities;

public class VendaItem
{
    public int VendaId { get; set; }
    [ForeignKey(nameof(VendaId))]
    public virtual Venda Venda { get; set; } = null!;
    public int ProdutoId { get; set; }
    [ForeignKey(nameof(ProdutoId))]
    public virtual Produto Produto { get; set; } = null!;
    public int Posicao { get; set; }
    public int Quantidade { get; set; } = 1;
    public decimal PrecoUnitario { get; set; }
}
