using minipdv.Domain.Entities.Base;

namespace minipdv.Domain.Entities;

public class Farmaceutico : AbstractUsuario
{
    public required string Crf { get; set; }
}
