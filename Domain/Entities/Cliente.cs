using System.ComponentModel.DataAnnotations.Schema;
using minipdv.Domain.Entities.Base;

namespace minipdv.Domain.Entities
{
    public class Cliente : Entity
    {
        public required string Nome { get; set; }
        public required string Cpf { get; set; }
        public int? ContatoId { get; set; }
        [ForeignKey(nameof(ContatoId))]
        public virtual Contato? Contato { get; set; }
    }
}
