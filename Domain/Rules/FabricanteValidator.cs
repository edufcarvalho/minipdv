using FluentValidation;
using minipdv.Domain.Entities;

namespace minipdv.Domain.Rules;

public class FabricanteValidator : AbstractValidator<Fabricante>
{
    public FabricanteValidator()
    {
        RuleFor(f => f.NomeFantasia)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(f => f.RazaoSocial)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(f => f.Cnpj)
            .NotEmpty()
            .MaximumLength(14)
            .Matches(@"^\d{14}$")
            .WithMessage("CNPJ deve conter exatamente 14 dígitos numéricos.");
    }
}
