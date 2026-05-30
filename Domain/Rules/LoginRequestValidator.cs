using FluentValidation;
using minipdv.Application.DTOs.Auth;

namespace minipdv.Domain.Rules;

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
