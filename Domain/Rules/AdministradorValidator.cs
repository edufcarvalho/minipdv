using FluentValidation;
using minipdv.Application.Validators;
using minipdv.Domain.Entities;

namespace minipdv.Domain.Rules;

public class AdministradorValidator : AbstractValidator<Administrador>
{
    public AdministradorValidator()
    {
        RuleFor(a => a.Nome)
            .MustBeRequiredName();

        RuleFor(a => a.Login)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(a => a.PasswordHash)
            .NotEmpty();
    }
}
