using FluentValidation;
using minipdv.Application.Validators;
using minipdv.Domain.Entities;

namespace minipdv.Domain.Rules;

public class ProdutoTipoValidator : AbstractValidator<ProdutoTipo>
{
    public ProdutoTipoValidator()
    {
        RuleFor(t => t.Nome)
            .MustBeRequiredName();
    }
}
