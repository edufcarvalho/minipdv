using FluentValidation;
using minipdv.Domain.Entities;

namespace minipdv.Domain.Rules;

public class VendaValidator : AbstractValidator<Venda>
{
    public VendaValidator()
    {
        RuleFor(v => v.ClienteId)
            .GreaterThan(0);

        RuleFor(v => v.VendaProdutoEstoques)
            .NotEmpty()
            .WithMessage("A venda deve conter pelo menos um produto");

        RuleForEach(v => v.VendaProdutoEstoques)
            .ChildRules(item =>
            {
                item.RuleFor(i => i.ProdutoId)
                    .GreaterThan(0);

                item.RuleFor(i => i.Lote)
                    .NotEmpty()
                    .MaximumLength(50);

                item.RuleFor(i => i.Quantidade)
                    .GreaterThan(0);
            });
    }
}
