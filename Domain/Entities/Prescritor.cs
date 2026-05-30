using minipdv.Domain.Entities.Base;
using minipdv.Domain.Enums;

namespace minipdv.Domain.Entities;

public class Prescritor : Entity
{
    public required string Numero { get; set; }
    public required string Nome { get; set; }
    public required Conselho Conselho { get; set; }
    public required UF Uf { get; set; }
}
