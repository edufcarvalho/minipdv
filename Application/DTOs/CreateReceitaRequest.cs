namespace minipdv.Application.DTOs;

public record CreateReceitaRequest(
    int PrescritorId,
    int PacienteId,
    int CompradorId,
    DateTime? DataReceita,
    DateTime? DataCadastro,
    List<ReceitaProdutoItem>? Produtos
);

public record UpdateReceitaRequest(
    int Id,
    int PrescritorId,
    int PacienteId,
    int CompradorId,
    DateTime? DataReceita,
    DateTime? DataCadastro,
    List<ReceitaProdutoItem>? Produtos
);

public record ReceitaProdutoItem(
    int ProdutoId,
    string Lote,
    int Quantidade = 1
);
