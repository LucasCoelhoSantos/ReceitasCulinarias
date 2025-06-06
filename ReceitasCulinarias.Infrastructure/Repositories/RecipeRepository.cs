using Microsoft.EntityFrameworkCore;
using ReceitasCulinarias.Domain.Entities;
using ReceitasCulinarias.Domain.Interfaces;
using ReceitasCulinarias.Infrastructure.Data;
using System;

namespace ReceitasCulinarias.Infrastructure.Repositories;

public class RecipeRepository : IRecipeRepository
{
    private readonly AppDbContext _context;

    public RecipeRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Recipe?> GetByIdAsync(Guid id)
    {
        return await _context.Recipes.FindAsync(id);
    }

    public async Task<IEnumerable<Recipe>> GetAllAsync()
    {
        return await _context.Recipes.ToListAsync();
    }

    public async Task CreateAsync(Recipe recipe)
    {
        ArgumentNullException.ThrowIfNull(recipe);

        await _context.Recipes.AddAsync(recipe);
    }

    public Task UpdateAsync(Recipe recipe)
    {
        ArgumentNullException.ThrowIfNull(recipe);

        _context.Recipes.Update(recipe);

        return Task.CompletedTask;
    }

    public Task DeleteAsync(Recipe recipe)
    {
        ArgumentNullException.ThrowIfNull(recipe);

        _context.Recipes.Remove(recipe);

        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(Guid id)
    {
        return _context.Recipes.AnyAsync(p => p.Id == id);
    }
}
