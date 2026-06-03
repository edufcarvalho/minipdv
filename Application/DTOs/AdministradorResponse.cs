namespace minipdv.Application.DTOs;

public record AdministradorResponse(
    int Id,
    string Nome,
    string Login,
    bool Ativo,
    int? ContatoId,
    DateTime CriadoEm,
    DateTime? AtualizadoEm
);
