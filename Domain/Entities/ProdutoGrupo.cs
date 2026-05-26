using minipdv.Domain.Entities.Base;

namespace minipdv.Domain.Entities;

public class ProdutoGrupo : Entity
{
    public required string Nome { get; set; }
    public bool Ativo { get; set; } = true;
    public virtual ICollection<Produto> Produtos { get; set; } = [];
}
