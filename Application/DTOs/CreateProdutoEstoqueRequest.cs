namespace minipdv.Application.DTOs;

public record CreateProdutoEstoqueRequest(
    int ProdutoId,
    string Lote,
    DateTime? Fabricacao,
    DateTime? Validade,
    int Quantidade
);

public record UpdateProdutoEstoqueRequest(
    int ProdutoId,
    string Lote,
    DateTime? Fabricacao,
    DateTime? Validade,
    int Quantidade
);
