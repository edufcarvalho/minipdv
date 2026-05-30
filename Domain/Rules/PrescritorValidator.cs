using FluentValidation;
using minipdv.Domain.Entities;

namespace minipdv.Domain.Rules;

public class PrescritorValidator : AbstractValidator<Prescritor>
{
    public PrescritorValidator()
    {
        RuleFor(p => p.Numero)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(p => p.Nome)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(p => p.Conselho)
            .IsInEnum();

        RuleFor(p => p.Uf)
            .IsInEnum();
    }
}
