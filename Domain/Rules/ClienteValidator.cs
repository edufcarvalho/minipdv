using FluentValidation;
using minipdv.Domain.Entities;

namespace minipdv.Domain.Rules;

public class ClienteValidator : AbstractValidator<Cliente>
{
    public ClienteValidator()
    {
        RuleFor(c => c.Nome)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(c => c.Cpf)
            .NotEmpty()
            .MaximumLength(14)
            .Matches(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$")
            .WithMessage("CPF deve seguir o formato 123.456.789-01");
    }
}
