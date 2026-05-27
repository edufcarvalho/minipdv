using minipdv.Domain.Entities;

namespace minipdv.Domain.Interfaces;

public interface IUsuarioRepository : IRepository<Usuario>
{
    Task<Usuario?> GetByLoginAsync(string login);
}
