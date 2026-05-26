using System.ComponentModel.DataAnnotations.Schema;
using minipdv.Domain.Entities.Base;

namespace minipdv.Domain.Entities;

public class Fabricante : Entity
{
    public required string NomeFantasia { get; set; }
    public required string RazaoSocial { get; set; }
    public required string Cnpj { get; set; }
    public int? ContatoId { get; set; }
    [ForeignKey(nameof(ContatoId))]
    public virtual Contato? Contato { get; set; }
}
