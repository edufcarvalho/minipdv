namespace minipdv.Application.DTOs;

public record CreateVendaRequest(
    int VendedorId,
    int ClienteId,
    List<VendaProdutoItem> Produtos,
    List<int>? ReceitaIds = null
);

public record VendaProdutoItem(
    int ProdutoId,
    int Quantidade = 1
);
