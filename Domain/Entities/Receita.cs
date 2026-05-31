using System.ComponentModel.DataAnnotations.Schema;
using minipdv.Domain.Entities.Base;

namespace minipdv.Domain.Entities;

public class Receita : Entity
{
    public required DateTime DataReceita { get; set; }
    public required DateTime DataCadastro { get; set; }
    public required int PrescritorId { get; set; }
    [ForeignKey(nameof(PrescritorId))]
    public required virtual Prescritor Prescritor { get; set; }
    public required int PacienteId { get; set; }
    [ForeignKey(nameof(PacienteId))]
    public required virtual Cliente Paciente { get; set; }
    public required int CompradorId { get; set; }
    [ForeignKey(nameof(CompradorId))]
    public required virtual Cliente Comprador { get; set; }
    public int? VendaId { get; set; }
    [ForeignKey(nameof(VendaId))]
    public virtual Venda? Venda { get; set; }
    public virtual ICollection<ReceitaProdutoEstoque> ReceitaProdutoEstoques { get; set; } = [];
    [NotMapped]
    public IEnumerable<Produto> Produtos =>
        ReceitaProdutoEstoques?.Select(rpe => rpe.ProdutoEstoque?.Produto).Where(p => p is not null)!;
}
