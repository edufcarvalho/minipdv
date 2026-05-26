using minipdv.Domain.Entities.Base;
namespace minipdv.Domain.Entities;

public class Farmaceutico : UsuarioBase
{
    public required string Crf { get; set; }
}
