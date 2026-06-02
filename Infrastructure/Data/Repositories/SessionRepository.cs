using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;
using minipdv.Infrastructure.Data.Context;

namespace minipdv.Infrastructure.Data.Repositories;

public class SessionRepository : Repository<Session>, ISessionRepository
{
    public SessionRepository(MiniPDVContext context, ILogger<SessionRepository> logger) : base(context, logger) { }

    public async Task<Session?> GetByTokenAsync(string token)
    {
        return await _dbSet.FirstOrDefaultAsync(s => s.Token == token);
    }

    public async Task RevokeAsync(int sessionId)
    {
        var session = await GetByIdAsync(sessionId);
        if (session is not null)
        {
            session.IsRevoked = true;
            await UpdateAsync(session);
        }
    }

    public async Task RevokeAllFromUserAsync(int usuarioId)
    {
        var sessions = await _dbSet
            .Where(s => s.UsuarioId == usuarioId && !s.IsRevoked)
            .ToListAsync();

        foreach (var session in sessions)
        {
            session.IsRevoked = true;
        }

        await _context.SaveChangesAsync();
    }
}
