using FluentValidation;
using minipdv.Domain.Entities;

namespace minipdv.Domain.Rules;

public class ProdutoTipoValidator : AbstractValidator<ProdutoTipo>
{
    public ProdutoTipoValidator()
    {
        RuleFor(t => t.Nome)
            .NotEmpty()
            .MaximumLength(200);
    }
}
