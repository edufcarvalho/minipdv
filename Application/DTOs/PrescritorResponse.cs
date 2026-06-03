namespace minipdv.Application.DTOs;

public record PrescritorResponse(
    int Id,
    string Nome,
    string Conselho,
    string UF,
    DateTime CriadoEm,
    DateTime? AtualizadoEm
);
