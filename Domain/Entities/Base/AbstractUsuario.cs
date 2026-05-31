namespace minipdv.Domain.Entities.Base;

using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using minipdv.Domain.Entities;

[JsonPolymorphic]
[JsonDerivedType(typeof(Usuario), "Usuario")]
[JsonDerivedType(typeof(Farmaceutico), "Farmaceutico")]
[JsonDerivedType(typeof(Administrador), "Administrador")]
public abstract class AbstractUsuario : Entity
{
    public required string Nome { get; set; }
    public required string Login { get; set; }
    public required string PasswordHash { get; set; }
    public bool Ativo { get; set; } = true;
    public required string TipoUsuario { get; set; }
    public int? ContatoId { get; set; }
    [ForeignKey(nameof(ContatoId))]
    public virtual Contato? Contato { get; set; }
}
