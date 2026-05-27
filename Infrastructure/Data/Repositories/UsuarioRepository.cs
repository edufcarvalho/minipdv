using Microsoft.EntityFrameworkCore;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;
using minipdv.Infrastructure.Data.Context;

namespace minipdv.Infrastructure.Data.Repositories;

public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(MiniPDVContext context) : base(context) { }

    public async Task<Usuario?> GetByLoginAsync(string login)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Login == login);
    }
}
