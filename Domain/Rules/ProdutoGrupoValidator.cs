using FluentValidation;
using minipdv.Application.Validators;
using minipdv.Domain.Entities;

namespace minipdv.Domain.Rules;

public class ProdutoGrupoValidator : AbstractValidator<ProdutoGrupo>
{
    public ProdutoGrupoValidator()
    {
        RuleFor(g => g.Nome)
            .MustBeRequiredName();
    }
}
