using FluentValidation;
using minipdv.Domain.Entities;

namespace minipdv.Domain.Rules;

public class ProdutoValidator : AbstractValidator<Produto>
{
    public ProdutoValidator()
    {
        RuleFor(p => p.Descricao)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(p => p.CodBarra)
            .NotEmpty()
            .MaximumLength(14)
            .Matches(@"^[0-9]+$")
            .WithMessage("Código de barras deve conter apenas caracteres numéricos");

        RuleFor(p => p.Dosagem)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(p => p.ProdutoGrupoId)
            .GreaterThan(0);

        RuleFor(p => p.PrincipioAtivoId)
            .GreaterThan(0);
    }
}
