using FluentValidation;
using minipdv.Domain.Entities;

namespace minipdv.Domain.Rules;

public class FarmaceuticoValidator : AbstractValidator<Farmaceutico>
{
    public FarmaceuticoValidator()
    {
        RuleFor(f => f.Crf)
            .NotEmpty()
            .MaximumLength(12)
            .Matches(@"^\d{4,6}/[A-Z]{2}$")
            .WithMessage("CRF deve seguir o formato (número)/(UF), ex: 12345/SP.");
    }
}
