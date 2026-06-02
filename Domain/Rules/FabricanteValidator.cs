using FluentValidation;
using minipdv.Application.Validators;
using minipdv.Domain.Entities;

namespace minipdv.Domain.Rules;

public class FabricanteValidator : AbstractValidator<Fabricante>
{
    public FabricanteValidator()
    {
        RuleFor(f => f.NomeFantasia)
            .MustBeRequiredName();

        RuleFor(f => f.RazaoSocial)
            .MustBeRequiredName();

        RuleFor(f => f.Cnpj)
            .NotEmpty()
            .MaximumLength(14)
            .Matches(@"^\d{14}$")
            .WithMessage(ValidationMessages.CnpjFormat);
    }
}
