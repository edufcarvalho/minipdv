namespace minipdv.Application.DTOs;

public record CreateProdutoControladoRequest(
    string Descricao,
    bool Ativo,
    int CodBarra,
    string Dosagem,
    int ProdutoGrupoId,
    int? FabricanteId,
    int PrincipioAtivoId,
    string RegistroMS,
    decimal Preco,
    string ClasseTerapeutica,
    string Lista
);

public record UpdateProdutoControladoRequest(
    int Id,
    string Descricao,
    bool Ativo,
    int CodBarra,
    string Dosagem,
    int ProdutoGrupoId,
    int? FabricanteId,
    int PrincipioAtivoId,
    string RegistroMS,
    decimal Preco,
    string ClasseTerapeutica,
    string Lista
);
