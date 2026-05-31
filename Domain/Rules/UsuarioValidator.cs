using FluentValidation;
using minipdv.Domain.Entities;

namespace minipdv.Domain.Rules;

public class UsuarioValidator : AbstractValidator<Usuario>
{
    public UsuarioValidator()
    {
        RuleFor(u => u.Nome)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(u => u.Login)
            .NotEmpty()
            .MaximumLength(100)
            .Matches(@"^[A-Za-z0-9]+$")
            .WithMessage("Login deve conter apenas letras e números");
    }
}
