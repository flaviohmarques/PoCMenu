using FluentValidation;
using MenuManagementAPI.Application.DTOs;

namespace MenuManagementAPI.Application.Validators;

/// <summary>
/// Validador para criação de Menu
/// </summary>
public class CreateMenuValidator : AbstractValidator<CreateMenuDto>
{
    public CreateMenuValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome do menu é obrigatório")
            .MaximumLength(255).WithMessage("O nome do menu deve ter no máximo 255 caracteres");

        RuleFor(x => x.Ordem)
            .GreaterThan(0).WithMessage("A ordem deve ser maior que zero");

        RuleFor(x => x.Icone)
            .NotEmpty().WithMessage("O ícone do menu é obrigatório")
            .MaximumLength(255).WithMessage("O ícone deve ter no máximo 255 caracteres");

        RuleFor(x => x.Descricao)
            .MaximumLength(1000).WithMessage("A descrição deve ter no máximo 1000 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Descricao));

        RuleFor(x => x.Status)
            .Must(status => status == "Ativo" || status == "Inativo")
            .WithMessage("O status deve ser 'Ativo' ou 'Inativo'");
    }
}

/// <summary>
/// Validador para atualização de Menu
/// </summary>
public class UpdateMenuValidator : AbstractValidator<UpdateMenuDto>
{
    public UpdateMenuValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome do menu é obrigatório")
            .MaximumLength(255).WithMessage("O nome do menu deve ter no máximo 255 caracteres");

        RuleFor(x => x.Ordem)
            .GreaterThan(0).WithMessage("A ordem deve ser maior que zero");

        RuleFor(x => x.Icone)
            .NotEmpty().WithMessage("O ícone do menu é obrigatório")
            .MaximumLength(255).WithMessage("O ícone deve ter no máximo 255 caracteres");

        RuleFor(x => x.Descricao)
            .MaximumLength(1000).WithMessage("A descrição deve ter no máximo 1000 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Descricao));

        RuleFor(x => x.Status)
            .Must(status => status == "Ativo" || status == "Inativo")
            .WithMessage("O status deve ser 'Ativo' ou 'Inativo'");
    }
}
