using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using minipdv.Domain.Entities.Base;
using minipdv.Domain.Interfaces;
using minipdv.Infrastructure.Data.Context;

namespace minipdv.Infrastructure.Data.Repositories;

public class AbstractUsuarioRepository : IAbstractUsuarioRepository
{
    private readonly MiniPDVContext _context;
    private readonly ILogger<AbstractUsuarioRepository> _logger;

    public AbstractUsuarioRepository(MiniPDVContext context, ILogger<AbstractUsuarioRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<AbstractUsuario?> GetByLoginAsync(string login)
    {
        return await _context.AbstractUsuarios.FirstOrDefaultAsync(u => u.Login == login);
    }
}
