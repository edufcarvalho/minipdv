using FluentValidation;
using minipdv.Domain.Entities;

namespace minipdv.Domain.Rules;

public class PrincipioAtivoValidator : AbstractValidator<PrincipioAtivo>
{
    public PrincipioAtivoValidator()
    {
        RuleFor(p => p.Nome)
            .NotEmpty()
            .MaximumLength(200);
    }
}
