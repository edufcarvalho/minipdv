namespace minipdv.Application.DTOs;

public record CreateReceitaRequest(
    int PrescritorId,
    int PacienteId,
    int CompradorId,
    List<ReceitaProdutoItem>? Produtos
);

public record UpdateReceitaRequest(
    int Id,
    int PrescritorId,
    int PacienteId,
    int CompradorId,
    List<ReceitaProdutoItem>? Produtos
);

public record ReceitaProdutoItem(
    int ProdutoId,
    string Lote,
    int Quantidade = 1
);
