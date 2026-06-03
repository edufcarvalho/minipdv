namespace minipdv.Application.DTOs;

public record VendaResponse(
    int Id,
    int VendedorId,
    int? ClienteId,
    DateTime CriadoEm,
    DateTime? AtualizadoEm
);
