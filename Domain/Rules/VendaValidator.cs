using FluentValidation;
using minipdv.Application.Validators;
using minipdv.Domain.Entities;

namespace minipdv.Domain.Rules;

public class VendaValidator : AbstractValidator<Venda>
{
    public VendaValidator()
    {
        RuleFor(v => v.VendedorId)
            .MustBeRequiredId();

        RuleFor(v => v.ClienteId)
            .MustBeRequiredId();

        RuleFor(v => v.VendaItens)
            .NotEmpty()
            .WithMessage(ValidationMessages.VendaMinimoProduto);

        RuleForEach(v => v.VendaItens)
            .ChildRules(item =>
            {
                item.RuleFor(i => i.ProdutoId)
                    .MustBeRequiredId();

                item.RuleFor(i => i.Quantidade)
                    .MustBeRequiredId();
            });
    }
}
