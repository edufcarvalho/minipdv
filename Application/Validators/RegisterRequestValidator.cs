using FluentValidation;
using minipdv.Application.DTOs.Auth;

namespace minipdv.Application.Validators;

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
            .Matches(@"^[A-Za-z0-9]+$")
            .WithMessage("Login deve conter apenas letras e números");

        RuleFor(r => r.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(128)
            .Matches(@"[A-Z]")
            .WithMessage("Senha deve conter pelo menos uma letra maiúscula")
            .Matches(@"[a-z]")
            .WithMessage("Senha deve conter pelo menos uma letra minúscula")
            .Matches(@"[0-9]")
            .WithMessage("Senha deve conter pelo menos um número");

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
