using Microsoft.EntityFrameworkCore;
using minipdv.Domain.Entities.Base;
using minipdv.Domain.Interfaces;
using minipdv.Infrastructure.Data.Context;

namespace minipdv.Infrastructure.Data.Repositories;

public class AbstractUsuarioRepository : IAbstractUsuarioRepository
{
    private readonly MiniPDVContext _context;

    public AbstractUsuarioRepository(MiniPDVContext context)
    {
        _context = context;
    }

    public async Task<AbstractUsuario?> GetByLoginAsync(string login)
    {
        return await _context.AbstractUsuarios.FirstOrDefaultAsync(u => u.Login == login);
    }
}
