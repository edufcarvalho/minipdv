namespace minipdv.Application.DTOs;

public record CreateVendaRequest(
    int ClienteId,
    int? ReceitaId,
    List<VendaProdutoItem> Produtos
);

public record VendaProdutoItem(
    int ProdutoId,
    string Lote,
    int Quantidade = 1
);
