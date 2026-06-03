namespace minipdv.Application.DTOs;

public record FarmaceuticoResponse(
    int Id,
    string Nome,
    string Login,
    bool Ativo,
    string Crf,
    int? ContatoId,
    DateTime CriadoEm,
    DateTime? AtualizadoEm
);
