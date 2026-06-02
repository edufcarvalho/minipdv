using FluentValidation;
using minipdv.Application.Validators;
using minipdv.Application.DTOs.Auth;

namespace minipdv.Application.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(r => r.Nome)
            .MustBeRequiredName();

        RuleFor(r => r.Login)
            .MustBeValidLogin();

        RuleFor(r => r.Password)
            .MustBeValidPassword();

        RuleFor(r => r.Tipo)
            .NotEmpty()
            .Must(t => t is "Usuario" or "Farmaceutico" or "Administrador")
            .WithMessage(ValidationMessages.TipoInvalid);

        RuleFor(r => r.Crf)
            .NotEmpty()
            .MaximumLength(12)
            .When(r => r.Tipo == "Farmaceutico")
            .WithMessage(ValidationMessages.CrfRequired);
    }
}
