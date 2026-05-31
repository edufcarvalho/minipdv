namespace minipdv.Application.DTOs;

public record CreateProdutoEstoqueRequest(
    int ProdutoId,
    string Lote,
    DateTime? Fabricacao,
    DateTime? Validade,
    int Quantidade,
    string? RegistroMS = null
);

public record UpdateProdutoEstoqueRequest(
    int ProdutoId,
    string Lote,
    DateTime? Fabricacao,
    DateTime? Validade,
    int Quantidade,
    string? RegistroMS = null
);
