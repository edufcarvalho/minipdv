using FluentValidation;
using Microsoft.Extensions.Logging;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;

namespace minipdv.Application.Services;

public class UsuarioService : CrudServiceBase<Usuario, IUsuarioRepository>, IUsuarioService
{
    public UsuarioService(IUsuarioRepository repository, IValidator<Usuario> validator, ILogger<UsuarioService> logger)
        : base(repository, validator, logger) { }

    protected override string EntityDisplayName => "Usuário";

    public async Task<Usuario?> GetByLoginAsync(string login)
    {
        return await Repository.GetByLoginAsync(login);
    }
}
