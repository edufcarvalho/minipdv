using FluentValidation;
using minipdv.Application.Validators;
using minipdv.Application.DTOs.Auth;

namespace minipdv.Application.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(r => r.Login)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(r => r.Password)
            .NotEmpty()
            .MaximumLength(128);
    }
}
