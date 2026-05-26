namespace minipdv.Domain.Entities;

public class ProdutoControlado : Produto
{
    public required new string RegistroMS { get; set; }
    public required string ClasseTerapeutica { get; set; }
    public required string Lista { get; set; }
}
