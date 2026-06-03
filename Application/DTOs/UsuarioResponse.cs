namespace minipdv.Application.DTOs;

public record UsuarioResponse(
    int Id,
    string Nome,
    string Login,
    bool Ativo,
    string TipoUsuario,
    int? ContatoId,
    DateTime CriadoEm,
    DateTime? AtualizadoEm
);
