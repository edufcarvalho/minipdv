using FluentValidation;
using minipdv.Domain.Entities;
using minipdv.Domain.Entities.Base;

namespace minipdv.Domain.Rules;

public class AbstractUsuarioValidator : AbstractValidator<AbstractUsuario>
{
    public AbstractUsuarioValidator()
    {
        RuleFor(u => u.Nome)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(u => u.Login)
            .NotEmpty()
            .MaximumLength(100)
            .Matches(@"^[A-z0-9]+$")
            .WithMessage("Login deve conter apenas letras e números");

        RuleFor(u => u.PasswordHash)
            .NotEmpty()
            .MaximumLength(128);
    }
}
