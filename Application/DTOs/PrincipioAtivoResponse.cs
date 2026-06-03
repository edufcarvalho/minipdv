namespace minipdv.Application.DTOs;

public record PrincipioAtivoResponse(
    int Id,
    string Descricao,
    DateTime CriadoEm,
    DateTime? AtualizadoEm
);
