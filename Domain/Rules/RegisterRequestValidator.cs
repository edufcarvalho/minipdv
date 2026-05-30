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

        RuleFor(r => r.Tipo)
            .NotEmpty()
            .Must(t => t is "Usuario" or "Farmaceutico" or "Administrador")
            .WithMessage("Tipo deve ser 'Usuario', 'Farmaceutico' ou 'Administrador'");

        RuleFor(r => r.Crf)
            .NotEmpty()
            .MaximumLength(12)
            .When(r => r.Tipo == "Farmaceutico")
            .WithMessage("CRF é obrigatório para Farmacêutico");
    }
}
