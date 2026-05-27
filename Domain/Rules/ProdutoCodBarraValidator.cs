using FluentValidation;
using minipdv.Domain.Entities;

namespace minipdv.Domain.Rules;

public class ProdutoCodBarraValidator : AbstractValidator<ProdutoCodBarra>
{
    public ProdutoCodBarraValidator()
    {
        RuleFor(p => p.CodBarra)
            .GreaterThan(10_000_000)
            .WithMessage("CodBarra deve ter pelo menos 8 dígitos");

        RuleFor(p => p.ProdutoId)
            .GreaterThan(0);
    }
}
