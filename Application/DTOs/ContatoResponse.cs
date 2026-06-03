namespace minipdv.Application.DTOs;

public record ContatoResponse(
    int Id,
    string? Email,
    string? Telefone,
    DateTime CriadoEm,
    DateTime? AtualizadoEm
);
