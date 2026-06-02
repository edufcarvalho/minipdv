using FluentValidation;
using minipdv.Application.Validators;
using minipdv.Domain.Entities;

namespace minipdv.Domain.Rules;

public class ClienteValidator : AbstractValidator<Cliente>
{
    public ClienteValidator()
    {
        RuleFor(c => c.Nome)
            .MustBeRequiredName();

        RuleFor(c => c.Cpf)
            .NotEmpty()
            .MaximumLength(14)
            .Matches(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$")
            .WithMessage(ValidationMessages.CpfFormat);
    }
}
