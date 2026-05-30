namespace minipdv.Domain.Entities.Base;

using System.ComponentModel.DataAnnotations.Schema;
using minipdv.Domain.Entities;

public abstract class AbstractUsuario : Entity
{
    public required string Nome { get; set; }
    public required string Login { get; set; }
    public required string PasswordHash { get; set; }
    public bool Ativo { get; set; } = true;
    public string TipoUsuario { get; set; } = string.Empty;
    public int? ContatoId { get; set; }
    [ForeignKey(nameof(ContatoId))]
    public virtual Contato? Contato { get; set; }
}
