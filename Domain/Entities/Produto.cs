using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using minipdv.Domain.Entities.Base;

namespace minipdv.Domain.Entities;

[JsonPolymorphic]
[JsonDerivedType(typeof(Produto))]
[JsonDerivedType(typeof(ProdutoControlado))]
public class Produto : Entity
{
    public required string Descricao { get; set; }
    public required bool Ativo { get; set; } = true;
    public required int CodBarra { get; set; }
    public required bool Controlado { get; set; } = false;
    public required string Dosagem { get; set; }
    public string? RegistroMS { get; set; }
    public required int ProdutoGrupoId { get; set; }
    [ForeignKey(nameof(ProdutoGrupoId))]
    public required virtual ProdutoGrupo Grupo { get; set; }
    public int? FabricanteId { get; set; }
    [ForeignKey(nameof(FabricanteId))]
    public virtual Fabricante? Fabricante { get; set; }
    public required int PrincipioAtivoId { get; set; }
    [ForeignKey(nameof(PrincipioAtivoId))]
    public required virtual PrincipioAtivo PrincipioAtivo { get; set; }
    public int Estoque { get; set; }
    public virtual ICollection<ProdutoCodBarra> CodBarras { get; set; } = [];
    public virtual ICollection<ProdutoEstoque> Estoques { get; set; } = [];
}
