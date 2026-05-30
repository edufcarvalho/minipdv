using System.ComponentModel.DataAnnotations.Schema;
using minipdv.Domain.Entities.Base;

namespace minipdv.Domain.Entities;

public class Venda : Entity
{
    public required int ClienteId { get; set; }
    [ForeignKey(nameof(ClienteId))]
    public required virtual Cliente Cliente { get; set; }
    public int? ReceitaId { get; set; }
    [ForeignKey(nameof(ReceitaId))]
    public virtual Receita? Receita { get; set; }
    public virtual ICollection<VendaProdutoEstoque> VendaProdutoEstoques { get; set; } = [];
}
