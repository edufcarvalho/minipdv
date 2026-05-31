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

        RuleFor(e => e.CodBarra)
            .GreaterThanOrEqualTo(10_000_000)
            .WithMessage("CodBarra deve ter pelo menos 8 caracteres");

        RuleFor(p => p.Dosagem)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(p => p.ProdutoGrupoId)
            .GreaterThan(0);

        RuleFor(p => p.PrincipioAtivoId)
            .GreaterThan(0);

        RuleFor(p => p.RegistroMS)
            .NotEmpty()
            .When(p => p.Controlado)
            .WithMessage("RegistroMS é obrigatório para medicamentos controlados");
    }
}
