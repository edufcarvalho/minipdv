namespace minipdv.Application.DTOs;

public record ProdutoResponse(
    int Id,
    string Descricao,
    bool Ativo,
    string CodBarra,
    bool Controlado,
    string? Dosagem,
    string? RegistroMS,
    decimal Preco,
    int? ProdutoGrupoId,
    int? FabricanteId,
    int? PrincipioAtivoId,
    DateTime CriadoEm,
    DateTime? AtualizadoEm
);
