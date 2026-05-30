using FluentValidation;
using minipdv.Domain.Entities;

namespace minipdv.Domain.Rules;

public class ProdutoEstoqueValidator : AbstractValidator<ProdutoEstoque>
{
    public ProdutoEstoqueValidator()
    {
        RuleFor(e => e.Lote)
            .NotEmpty()
            .MaximumLength(50)
            .Matches(@"^[A-z0-9\-]+$")
            .WithMessage("Lote deve conter apenas caracteres alfanuméricos e traços");

        RuleFor(e => e.ProdutoId)
            .GreaterThan(0);

        RuleFor(e => e.Quantidade)
            .GreaterThanOrEqualTo(0);

        RuleFor(e => e.Validade)
            .GreaterThan(e => e.Fabricacao)
            .When(e => e.Fabricacao.HasValue && e.Validade.HasValue)
            .WithMessage("Data de validade deve ser posterior à data de fabricação");

        RuleFor(e => e.Validade)
            .NotEmpty()
            .When(e => e.Produto.Controlado)
            .WithMessage("Data de validade é obrigatória em medicamentos controlados");

        RuleFor(e => e.Fabricacao)
        .NotEmpty()
        .When(e => e.Produto.Controlado)
        .WithMessage("Data de fabricação é obrigatória em medicamentos controlados");
    }
}
