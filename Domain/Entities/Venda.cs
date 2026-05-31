using System.ComponentModel.DataAnnotations.Schema;
using minipdv.Domain.Entities.Base;

namespace minipdv.Domain.Entities;

public class Venda : Entity
{
    public required int ClienteId { get; set; }
    [ForeignKey(nameof(ClienteId))]
    public required virtual Cliente Cliente { get; set; }
    public required int VendedorId { get; set; }
    [ForeignKey(nameof(VendedorId))]
    public required virtual Usuario Vendedor { get; set; }
    public DateTime? CanceladoEm { get; set; }
    public virtual ICollection<VendaProdutoEstoque> VendaProdutoEstoques { get; set; } = [];
    public virtual ICollection<Receita> Receitas { get; set; } = [];
}
