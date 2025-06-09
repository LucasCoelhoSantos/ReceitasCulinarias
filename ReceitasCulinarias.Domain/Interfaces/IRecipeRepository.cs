using ReceitasCulinarias.Domain.Entities;

namespace ReceitasCulinarias.Domain.Interfaces;

public interface IRecipeRepository
{
    Task<Recipe?> GetByIdAsync(Guid id);
    Task<IEnumerable<Recipe>> GetAllAsync();
    Task CreateAsync(Recipe recipe);
    Task UpdateAsync(Recipe recipe);
    Task DeleteAsync(Recipe recipe);
    Task<bool> ExistsAsync(Guid id);
}
