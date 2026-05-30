using FluentValidation;
using minipdv.Domain.Entities;

namespace minipdv.Domain.Rules;

public class ReceitaValidator : AbstractValidator<Receita>
{
    public ReceitaValidator()
    {
        RuleFor(r => r.PrescritorId)
            .GreaterThan(0);

        RuleFor(r => r.PacienteId)
            .GreaterThan(0);

        RuleFor(r => r.CompradorId)
            .GreaterThan(0);
    }
}
