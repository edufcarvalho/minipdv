namespace minipdv.Application.DTOs;

public record ProdutoGrupoResponse(
    int Id,
    string Descricao,
    DateTime CriadoEm,
    DateTime? AtualizadoEm
);
