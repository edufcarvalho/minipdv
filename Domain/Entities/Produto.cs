using System.ComponentModel.DataAnnotations.Schema;
using minipdv.Domain.Entities.Base;

namespace minipdv.Domain.Entities;

public class Produto : Entity
{
    public required string Descricao { get; set; }
    public int ProdutoGrupoId { get; set; }
    public required bool Ativo { get; set; } = true;
    [ForeignKey(nameof(ProdutoGrupoId))]
    public required virtual ProdutoGrupo Grupo { get; set; }
}
