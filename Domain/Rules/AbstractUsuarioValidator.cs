using FluentValidation;
using minipdv.Application.Validators;
using minipdv.Domain.Entities;
using minipdv.Domain.Entities.Base;

namespace minipdv.Domain.Rules;

public class AbstractUsuarioValidator : AbstractValidator<AbstractUsuario>
{
    public AbstractUsuarioValidator()
    {
        RuleFor(u => u.Nome)
            .MustBeRequiredName();

        RuleFor(u => u.Login)
            .MustBeValidLogin();

        RuleFor(u => u.PasswordHash)
            .NotEmpty()
            .MaximumLength(128);

        RuleFor(u => u.TipoUsuario)
            .MustBeValidTipoUsuario();
    }
}
