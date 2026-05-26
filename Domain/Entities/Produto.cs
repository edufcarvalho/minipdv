using System.ComponentModel.DataAnnotations.Schema;

namespace minipdv.Domain.Entities;

public class Produto
{
    public int Id { get; set; }
    public required string Descricao { get; set; }
    public required bool Ativo { get; set; } = true;
    public DateTime CriadoEm { get; set; }
    public DateTime? AtualizadoEm { get; set; }
    public int ProdutoGrupoId { get; set; }
    [ForeignKey(nameof(ProdutoGrupoId))]
    public required virtual ProdutoGrupo Grupo { get; set; }
}
