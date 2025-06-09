using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReceitasCulinarias.Application.Recipes.DTOs;
using ReceitasCulinarias.Application.Recipes.Interfaces;

namespace ReceitasCulinarias.API.Controllers;

[ApiController]
[Route("api/v1/recipes")]
[Authorize]
public class RecipesController : ControllerBase
{
    private readonly IRecipeService _recipeService;
    private readonly ILogger<RecipesController> _logger;

    public RecipesController(IRecipeService recipeService, ILogger<RecipesController> logger)
    {
        _recipeService = recipeService ?? throw new ArgumentNullException(nameof(recipeService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // GET: api/v1/recipes
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RecipeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("Requisição recebida para GetAll receitas.");
        var recipes = await _recipeService.GetAllAsync();
        _logger.LogInformation("Retornando {Count} receitas.", recipes.Count());
        return Ok(recipes);
    }

    // GET: api/v1/recipes/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(RecipeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        _logger.LogInformation("Requisição recebida para GetById receita com ID: {RecipeId}", id);
        var recipe = await _recipeService.GetByIdAsync(id);
        if (recipe == null)
        {
            _logger.LogWarning("Receita com ID: {RecipeId} não encontrado na requisição GetById.", id);
            // Poderíamos criar uma exceção customizada "NotFoundException" e deixá-la ser tratada pelo middleware global.
            return NotFound($"Receita com ID {id} não encontrado.");
        }
        _logger.LogInformation("Receita com ID: {RecipeId} encontrado e retornado.", id);
        return Ok(recipe);
    }

    // POST: api/v1/recipes
    [HttpPost]
    [ProducesResponseType(typeof(RecipeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateRecipeRequestDto createRecipeDto)
    {
        _logger.LogInformation("Requisição recebida para Create receita com nome: {RecipeName}", createRecipeDto.Name);
        var newRecipe = await _recipeService.CreateAsync(createRecipeDto);
        _logger.LogInformation("Receita criada com sucesso. ID: {RecipeId}, Nome: {RecipeName}", newRecipe.Id, newRecipe.Name);
        return CreatedAtAction(nameof(GetById), new { id = newRecipe.Id }, newRecipe);
    }

    // PUT: api/v1/recipes/{id}
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRecipeRequestDto updateRecipeDto)
    {
        _logger.LogInformation("Requisição recebida para Update receita com ID: {RecipeId}", id);
        var success = await _recipeService.UpdateAsync(id, updateRecipeDto);
        if (!success)
        {
            _logger.LogWarning("Receita com ID: {RecipeId} não encontrada para Update.", id);
            return NotFound($"Receita com ID {id} não encontrada para atualização.");
        }
        _logger.LogInformation("Receita com ID: {RecipeId} atualizada com sucesso.", id);
        return NoContent();
    }

    // DELETE: api/v1/recipes/{id}
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        _logger.LogInformation("Requisição recebida para Delete receita com ID: {RecipeId}", id);
        var success = await _recipeService.DeleteAsync(id);
        if (!success)
        {
            _logger.LogWarning("Receita com ID: {RecipeId} não encontrada para Delete.", id);
            return NotFound($"Receita com ID {id} não encontrada para exclusão.");
        }
        _logger.LogInformation("Receita com ID: {RecipeId} excluída com sucesso.", id);
        return NoContent();
    }
}
