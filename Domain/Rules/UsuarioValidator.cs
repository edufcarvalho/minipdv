using FluentValidation;
using minipdv.Domain.Entities;
using minipdv.Domain.Rules;

namespace minipdv.Domain.Rules;

public class UsuarioValidator : AbstractValidator<Usuario>
{
    public UsuarioValidator()
    {
        Include(new AbstractUsuarioValidator());
    }
}
