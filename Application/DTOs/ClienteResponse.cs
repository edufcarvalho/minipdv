namespace minipdv.Application.DTOs;

public record ClienteResponse(
    int Id,
    string Nome,
    int? ContatoId,
    DateTime CriadoEm,
    DateTime? AtualizadoEm
);
