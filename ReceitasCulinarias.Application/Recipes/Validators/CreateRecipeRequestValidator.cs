using FluentValidation;
using ReceitasCulinarias.Application.Recipes.DTOs;

namespace ReceitasCulinarias.Application.Recipes.Validators;

public class CreateRecipeRequestValidator : AbstractValidator<CreateRecipeRequestDto>
{
    public CreateRecipeRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome da receita é obrigatória.")
            .Length(3, 200).WithMessage("O nome da receita deve ter entre 3 e 200 caracteres.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("A descrição da receita é obrigatória.");

        RuleFor(x => x.Ingredients)
            .NotEmpty().WithMessage("Os ingredientes da receita é obrigatório.");

        RuleFor(x => x.Instructions)
            .NotEmpty().WithMessage("O modo de preparo da receita é obrigatório.");

        RuleFor(x => x.PrepTimeMinutes)
            .GreaterThan(0).WithMessage("O tempo de preparo da receita deve ser maior que zero.");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("A categoria da receita é obrigatória.");

        RuleFor(x => x.ImageUrl)
            .NotEmpty().WithMessage("A imagem da receita é obrigatória.");
    }
}
