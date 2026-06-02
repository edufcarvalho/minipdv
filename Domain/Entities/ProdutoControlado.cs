namespace minipdv.Domain.Entities;

public class ProdutoControlado : Produto
{
    public required string ClasseTerapeutica { get; set; }
    public required string Lista { get; set; }
    public override int Estoque
    {
        get => Estoques.Sum(e => e.Quantidade);
        set { }
    }
}
