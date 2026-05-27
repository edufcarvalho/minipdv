using FluentValidation;
using minipdv.Domain.Entities;

namespace minipdv.Domain.Rules;

public class ContatoValidator : AbstractValidator<Contato>
{
    public ContatoValidator()
    {
        RuleFor(c => c.Email)
            .EmailAddress()
            .MaximumLength(200)
            .When(c => !string.IsNullOrEmpty(c.Email));

        RuleFor(c => c.Telefone)
            .MaximumLength(20)
            .Matches(@"^\+?[1-9]\d{7,14}$")
            .WithMessage("Número de telefone inválido")
            .When(c => !string.IsNullOrEmpty(c.Telefone));
    }
}
