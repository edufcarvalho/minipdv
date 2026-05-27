using FluentValidation;
using minipdv.Domain.Entities;

namespace minipdv.Domain.Rules;

public class ProdutoGrupoValidator : AbstractValidator<ProdutoGrupo>
{
    public ProdutoGrupoValidator()
    {
        RuleFor(g => g.Nome)
            .NotEmpty()
            .MaximumLength(200);
    }
}
