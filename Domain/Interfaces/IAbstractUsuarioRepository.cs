using minipdv.Domain.Entities.Base;

namespace minipdv.Domain.Interfaces;

public interface IAbstractUsuarioRepository
{
    Task<AbstractUsuario?> GetByLoginAsync(string login);
}
