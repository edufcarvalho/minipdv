using FluentValidation;

namespace minipdv.Application.Validators;

public static class ValidatorExtensions
{
    public static IRuleBuilderOptions<T, string> MustBeRequiredName<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.NotEmpty().MaximumLength(200);
    }

    public static IRuleBuilderOptions<T, string> MustBeValidLogin<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .MaximumLength(100)
            .Matches(@"^[A-Za-z0-9]+$")
            .WithMessage(ValidationMessages.LoginAlphanumericOnly);
    }

    public static IRuleBuilderOptions<T, string> MustBeValidPassword<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(128)
            .Matches(@"[A-Z]").WithMessage(ValidationMessages.PasswordUppercase)
            .Matches(@"[a-z]").WithMessage(ValidationMessages.PasswordLowercase)
            .Matches(@"[0-9]").WithMessage(ValidationMessages.PasswordDigit);
    }

    public static IRuleBuilderOptions<T, int> MustBeRequiredId<T>(
        this IRuleBuilder<T, int> ruleBuilder)
    {
        return ruleBuilder.GreaterThan(0);
    }

    public static IRuleBuilderOptions<T, string> MustBeValidTipoUsuario<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .MaximumLength(21)
            .Must(t => t is "Usuario" or "Farmaceutico" or "Administrador")
            .WithMessage(ValidationMessages.TipoUsuarioInvalid);
    }
}
