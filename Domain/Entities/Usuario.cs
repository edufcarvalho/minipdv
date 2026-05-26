namespace minipdv.Domain.Entities;

public class Usuario
{
    public int Id { get; set; }
    public required string Nome { get; set; }
    public required string Login { get; set; }
    public required string PasswordHash { get; set; }
    public required bool Ativo { get; set; } = true;
    public DateTime CriadoEm { get; set; }
    public DateTime? AtualizadoEm { get; set; }
}
