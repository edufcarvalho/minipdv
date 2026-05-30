using minipdv.Domain.Entities;

namespace minipdv.Domain.Interfaces;

public interface ISessionRepository : IRepository<Session>
{
    Task<Session?> GetByTokenAsync(string token);
    Task RevokeAsync(int sessionId);
    Task RevokeAllFromUserAsync(int usuarioId);
}
