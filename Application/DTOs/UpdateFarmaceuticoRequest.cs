namespace minipdv.Application.DTOs;

public record UpdateFarmaceuticoRequest(
    string Nome,
    string Login,
    bool Ativo,
    string Crf,
    int? ContatoId,
    string? Password = null
);
