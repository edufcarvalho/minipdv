using FluentValidation;
using minipdv.Application.Validators;
using minipdv.Domain.Entities;

namespace minipdv.Domain.Rules;

public class ReceitaValidator : AbstractValidator<Receita>
{
    public ReceitaValidator()
    {
        RuleFor(r => r.PrescritorId)
            .MustBeRequiredId();

        RuleFor(r => r.PacienteId)
            .MustBeRequiredId();

        RuleFor(r => r.CompradorId)
            .MustBeRequiredId();
    }
}
