namespace minipdv.Application.DTOs;

public record ProdutoEstoqueResponse(
    int ProdutoId,
    string Lote,
    DateTime? Fabricacao,
    DateTime? Validade,
    int Quantidade,
    string? RegistroMS,
    DateTime CriadoEm,
    DateTime? AtualizadoEm
);
