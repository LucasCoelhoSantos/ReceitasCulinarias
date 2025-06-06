using ReceitasCulinarias.Application.Recipes.DTOs;

namespace ReceitasCulinarias.Application.Recipes.Interfaces;

public interface IRecipeService
{
    Task<RecipeDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<RecipeDto>> GetAllAsync();
    Task<RecipeDto> CreateAsync(CreateRecipeRequestDto recipeDto);
    Task<bool> UpdateAsync(Guid id, UpdateRecipeRequestDto recipeDto);
    Task<bool> DeleteAsync(Guid id);
}
