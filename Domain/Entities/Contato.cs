using minipdv.Domain.Entities.Base;

namespace minipdv.Domain.Entities;

public class Contato : Entity
{
    public string? Email { get; set; }
    public string? Telefone { get; set; }
}
