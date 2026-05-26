namespace minipdv.Domain.Entities.Base;

public abstract class UsuarioBase : Entity
{
    public required string Nome { get; set; }
    public required string Login { get; set; }
    public required string PasswordHash { get; set; }
    public required bool Ativo { get; set; } = true;
}
