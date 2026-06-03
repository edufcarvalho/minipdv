namespace minipdv.Application.DTOs;

public record UpdateAdministradorRequest(
    string Nome,
    string Login,
    bool Ativo,
    int? ContatoId,
    string? Password = null
);
