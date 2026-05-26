using System.ComponentModel.DataAnnotations.Schema;

namespace minipdv.Domain.Entities;

public class ProdutoCodBarra
{
    public required int CodBarra { get; set; }
    public required int ProdutoId { get; set; }
    [ForeignKey(nameof(ProdutoId))]
    public required virtual Produto Produto { get; set; }
    public DateTime CriadoEm { get; set; }
}
