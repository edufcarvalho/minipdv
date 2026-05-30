using FluentValidation;
using minipdv.Application.DTOs.Auth;

namespace minipdv.Domain.Rules;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(r => r.Nome)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(r => r.Login)
            .NotEmpty()
            .MaximumLength(100)
            .Matches(@"^[A-z0-9]+$")
            .WithMessage("Login deve conter apenas letras e números");

        RuleFor(r => r.Password)
            .NotEmpty()
            .MinimumLength(6)
            .MaximumLength(128);
    }
}
