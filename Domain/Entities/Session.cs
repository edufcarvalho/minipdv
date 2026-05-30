using minipdv.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace minipdv.Domain.Entities;

public class Session : Entity
{
    public int UsuarioId { get; set; }
    public required string Token { get; set; }
    public required string DeviceInfo { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    [ForeignKey(nameof(UsuarioId))]
    public virtual Usuario Usuario { get; set; } = null!;
}
