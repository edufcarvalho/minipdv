using FluentValidation;
using minipdv.Domain.Entities;

namespace minipdv.Domain.Rules;

public class ProdutoControladoValidator : AbstractValidator<ProdutoControlado>
{
    public ProdutoControladoValidator()
    {
        RuleFor(p => p.RegistroMS)
            .NotEmpty()
            .MaximumLength(20)
            .Matches(@"^\d\.\d{4}\.\d{4}\.\d{3}-\d$$")
            .WithMessage("Registro MS deve seguir o formato X.XXXX.XXXX.XXX-X");

        RuleFor(p => p.ClasseTerapeutica)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(p => p.Lista)
            .NotEmpty()
            .MaximumLength(50);
    }
}
