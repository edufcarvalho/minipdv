namespace minipdv.Application.DTOs;

public record UpdateUsuarioRequest(
    string Nome,
    string Login,
    bool Ativo,
    string TipoUsuario,
    int? ContatoId,
    string? Password = null
);
