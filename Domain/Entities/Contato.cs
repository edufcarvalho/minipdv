using minipdv.Domain.Entities.Base;

namespace minipdv.Domain.Entities;

public class Contato : Entity
{
    public required string? Email { get; set; }
    public required string? Telefone { get; set; }
}
