namespace minipdv.Application.DTOs;

public record FabricanteResponse(
    int Id,
    string Nome,
    int? ContatoId,
    DateTime CriadoEm,
    DateTime? AtualizadoEm
);
