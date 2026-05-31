using FluentValidation;
using minipdv.Domain.Entities;

namespace minipdv.Domain.Rules;

public class VendaValidator : AbstractValidator<Venda>
{
    public VendaValidator()
    {
        RuleFor(v => v.VendedorId)
            .GreaterThan(0);

        RuleFor(v => v.ClienteId)
            .GreaterThan(0);

        RuleFor(v => v.VendaItens)
            .NotEmpty()
            .WithMessage("A venda deve conter pelo menos um produto");

        RuleForEach(v => v.VendaItens)
            .ChildRules(item =>
            {
                item.RuleFor(i => i.ProdutoId)
                    .GreaterThan(0);

                item.RuleFor(i => i.Quantidade)
                    .GreaterThan(0);
            });
    }
}
