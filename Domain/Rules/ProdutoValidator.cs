using FluentValidation;
using minipdv.Application.Validators;
using minipdv.Domain.Entities;

namespace minipdv.Domain.Rules;

public class ProdutoValidator : AbstractValidator<Produto>
{
    public ProdutoValidator()
    {
        RuleFor(p => p.Descricao)
            .MustBeRequiredName();

        RuleFor(e => e.CodBarra)
            .GreaterThanOrEqualTo(10_000_000)
            .WithMessage(ValidationMessages.CodBarraMinLength);

        RuleFor(p => p.Dosagem)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(p => p.ProdutoGrupoId)
            .MustBeRequiredId();

        RuleFor(p => p.PrincipioAtivoId)
            .MustBeRequiredId();

        RuleFor(p => p.RegistroMS)
            .NotEmpty()
            .When(p => p.Controlado)
            .WithMessage(ValidationMessages.RegistroMSRequired);
    }
}
