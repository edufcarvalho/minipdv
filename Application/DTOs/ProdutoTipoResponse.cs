namespace minipdv.Application.DTOs;

public record ProdutoTipoResponse(
    int Id,
    string Descricao,
    DateTime CriadoEm,
    DateTime? AtualizadoEm
);
