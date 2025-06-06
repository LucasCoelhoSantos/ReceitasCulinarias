using FluentValidation;
using Microsoft.Extensions.Logging;
using ReceitasCulinarias.Application.Recipes.DTOs;
using ReceitasCulinarias.Application.Recipes.Interfaces;
using ReceitasCulinarias.Domain.Entities;
using ReceitasCulinarias.Domain.Interfaces;

namespace ReceitasCulinarias.Application.Recipes.Services;

public class RecipeService : IRecipeService
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateRecipeRequestDto> _createRecipeValidator;
    private readonly IValidator<UpdateRecipeRequestDto> _updateRecipeValidator;
    private readonly ILogger<RecipeService> _logger;

    public RecipeService(
        IRecipeRepository recipeRepository,
        IUnitOfWork unitOfWork,
        IValidator<CreateRecipeRequestDto> createRecipeValidator,
        IValidator<UpdateRecipeRequestDto> updateRecipeValidator,
        ILogger<RecipeService> logger)
    {
        _recipeRepository = recipeRepository ?? throw new ArgumentNullException(nameof(recipeRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _createRecipeValidator = createRecipeValidator ?? throw new ArgumentNullException(nameof(createRecipeValidator));
        _updateRecipeValidator = updateRecipeValidator ?? throw new ArgumentNullException(nameof(updateRecipeValidator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<RecipeDto?> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Buscando receita com ID: {RecipeId}", id);

        var recipes = await _recipeRepository.GetByIdAsync(id);
        if (recipes == null)
        {
            _logger.LogWarning("Receita com ID: {RecipeId} não encontrado.", id);
            return null;
        }

        _logger.LogInformation("Receita com ID: {RecipeId} encontrado.", id);
        // Mapeamento manual Entidade -> DTO
        return new RecipeDto
        {
            Id = recipes.Id,
            CreatedDate = recipes.CreatedDate,
            UpdatedDate = recipes.UpdatedDate,
            Name = recipes.Name,
            Description = recipes.Description,
            Ingredients = recipes.Ingredients,
            Instructions = recipes.Instructions,
            PrepTimeMinutes = recipes.PrepTimeMinutes,
            Category = recipes.Category,
            ImageUrl = recipes.ImageUrl
        };
    }

    public async Task<IEnumerable<RecipeDto>> GetAllAsync()
    {
        _logger.LogInformation("Buscando todos as receitas.");

        var recipes = await _recipeRepository.GetAllAsync();

        _logger.LogInformation("Encontrados {Count} receitas.", recipes.Count());
        // Mapeamento manual Entidade -> DTO
        return recipes.Select(p => new RecipeDto
        {
            Id = p.Id,
            CreatedDate = p.CreatedDate,
            UpdatedDate = p.UpdatedDate,
            Name = p.Name,
            Description = p.Description,
            Ingredients = p.Ingredients,
            Instructions = p.Instructions,
            PrepTimeMinutes = p.PrepTimeMinutes,
            Category = p.Category,
            ImageUrl = p.ImageUrl
        }).ToList();
    }

    public async Task<RecipeDto> CreateAsync(CreateRecipeRequestDto recipeDto)
    {
        _logger.LogInformation("Tentando criar receita com nome: {RecipeName}", recipeDto.Name);

        var validationResult = await _createRecipeValidator.ValidateAsync(recipeDto);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validação falhou para criação da receita: {RecipeName}. Erros: {ValidationErrors}", recipeDto.Name, validationResult.Errors.Select(e => e.ErrorMessage));
            // É uma boa prática encapsular isso em uma exceção customizada de validação.
            throw new ValidationException(validationResult.Errors);
        }

        var recipe = new Recipe(recipeDto.Name, recipeDto.Description, recipeDto.Ingredients, recipeDto.Instructions, recipeDto.PrepTimeMinutes, recipeDto.Category, recipeDto.ImageUrl);

        await _recipeRepository.CreateAsync(recipe);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Receita criada com sucesso. ID: {RecipeId}, Nome: {RecipeName}", recipe.Id, recipe.Name);
        // Mapeamento manual Entidade -> DTO
        return new RecipeDto
        {
            Id = recipe.Id,
            CreatedDate = recipe.CreatedDate,
            UpdatedDate = recipe.UpdatedDate,
            Name = recipe.Name,
            Description = recipe.Description,
            Ingredients = recipe.Ingredients,
            Instructions = recipe.Instructions,
            PrepTimeMinutes = recipe.PrepTimeMinutes,
            Category = recipe.Category,
            ImageUrl = recipe.ImageUrl
        };
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateRecipeRequestDto recipeDto)
    {
        _logger.LogInformation("Tentando atualizar receita com ID: {RecipeId}", id);

        var validationResult = await _updateRecipeValidator.ValidateAsync(recipeDto);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validação falhou para atualização da receita ID: {RecipeId}. Erros: {ValidationErrors}", id, validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ValidationException(validationResult.Errors);
        }

        var recipeExisting = await _recipeRepository.GetByIdAsync(id);
        if (recipeExisting == null)
        {
            _logger.LogWarning("Receita com ID: {RecipeId} não encontrada para atualização.", id);
            return false; // Ou lançar uma NotFoundException customizada
        }

        recipeExisting.AtualizarDetalhes(recipeDto.Name, recipeDto.Description, recipeDto.Ingredients, recipeDto.Instructions, recipeDto.PrepTimeMinutes, recipeDto.Category, recipeDto.ImageUrl);

        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Receita com ID: {RecipeId} atualizado com sucesso.", id);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        _logger.LogInformation("Tentando excluir receita com ID: {RecipeId}", id);

        var recipeExisting = await _recipeRepository.GetByIdAsync(id);
        if (recipeExisting == null)
        {
            _logger.LogWarning("Receita com ID: {RecipeId} não encontrada para exclusão.", id);
            return false; // Ou lançar uma NotFoundException customizada
        }

        await _recipeRepository.DeleteAsync(recipeExisting);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Receita com ID: {RecipeId} excluída com sucesso.", id);
        return true;
    }
}
