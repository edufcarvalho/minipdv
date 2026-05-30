using FluentValidation;
using minipdv.Domain.Entities;

namespace minipdv.Domain.Rules;

public class AdministradorValidator : AbstractValidator<Administrador>
{
    public AdministradorValidator()
    {
        RuleFor(a => a.Nome)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(a => a.Login)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(a => a.PasswordHash)
            .NotEmpty();
    }
}
