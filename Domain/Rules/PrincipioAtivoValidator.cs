using FluentValidation;
using minipdv.Application.Validators;
using minipdv.Domain.Entities;

namespace minipdv.Domain.Rules;

public class PrincipioAtivoValidator : AbstractValidator<PrincipioAtivo>
{
    public PrincipioAtivoValidator()
    {
        RuleFor(p => p.Nome)
            .MustBeRequiredName();
    }
}
