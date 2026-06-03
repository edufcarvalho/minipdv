namespace minipdv.Application.DTOs;

public record CreateProdutoRequest(
    string Descricao,
    bool Ativo,
    int CodBarra,
    bool Controlado,
    string Dosagem,
    string? RegistroMS,
    decimal Preco,
    int ProdutoGrupoId,
    int? FabricanteId,
    int PrincipioAtivoId
);

public record UpdateProdutoRequest(
    int Id,
    string Descricao,
    bool Ativo,
    int CodBarra,
    bool Controlado,
    string Dosagem,
    string? RegistroMS,
    decimal Preco,
    int ProdutoGrupoId,
    int? FabricanteId,
    int PrincipioAtivoId
);
