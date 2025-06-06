using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using ReceitasCulinarias.Application.Recipes.Services;
using ReceitasCulinarias.Application.Recipes.DTOs;
using ReceitasCulinarias.Domain.Entities;
using ReceitasCulinarias.Domain.Interfaces;

namespace ReceitasCulinarias.Application.Tests.Recipes.Services;

public class RecipeServiceTests
{
    private readonly Mock<IRecipeRepository> _mockRecipeRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IValidator<CreateRecipeRequestDto>> _mockCreateRecipeValidator;
    private readonly Mock<IValidator<UpdateRecipeRequestDto>> _mockUpdateRecipeValidator;
    private readonly Mock<ILogger<RecipeService>> _mockLogger;
    private readonly RecipeService _recipeService;

    public RecipeServiceTests()
    {
        _mockRecipeRepository = new Mock<IRecipeRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockCreateRecipeValidator = new Mock<IValidator<CreateRecipeRequestDto>>();
        _mockUpdateRecipeValidator = new Mock<IValidator<UpdateRecipeRequestDto>>();
        _mockLogger = new Mock<ILogger<RecipeService>>();

        _recipeService = new RecipeService(
            _mockRecipeRepository.Object,
            _mockUnitOfWork.Object,
            _mockCreateRecipeValidator.Object,
            _mockUpdateRecipeValidator.Object,
            _mockLogger.Object
        );
    }

    #region Tests for CreatedAsync

    [Fact]
    public async Task CreateAsync_WithValidDto_ShouldReturnRecipeDtoAndCreateRecipe()
    {
        // Arrange
        var createRecipeDto = new CreateRecipeRequestDto { Name = "Receita Teste", Description = "Descrição Teste", Ingredients = "Tomate", Instructions = "Fatiar", PrepTimeMinutes = 5, Category = "Salada", ImageUrl = "url.com.br" };
        var recipe = new Recipe(createRecipeDto.Name, createRecipeDto.Description, createRecipeDto.Ingredients, createRecipeDto.Instructions, createRecipeDto.PrepTimeMinutes, createRecipeDto.Category, createRecipeDto.ImageUrl);

        // Configura o mock do validador para retornar um resultado válido
        _mockCreateRecipeValidator
            .Setup(v => v.ValidateAsync(It.IsAny<CreateRecipeRequestDto>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(new ValidationResult()); // Resultado de validação válido (sem erros)

        // Configura o mock do repositório para AddAsync
        _mockRecipeRepository.Setup(r => r.CreateAsync(It.IsAny<Recipe>()))
            .Returns(Task.CompletedTask) // Ou .Callback<Recipe>(p => { /* pode inspecionar 'p' aqui */ });
            .Verifiable(); // Marca para verificação posterior

        // Configura o mock do UnitOfWork para SaveChangesAsync
        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(1) // Simula que 1 registro foi afetado
            .Verifiable();

        // Act
        var resultDto = await _recipeService.CreateAsync(createRecipeDto);

        // Assert (Verificar)
        resultDto.Should().NotBeNull();
        resultDto.Id.Should().NotBeEmpty();
        resultDto.Name.Should().Be(createRecipeDto.Name);
        resultDto.Description.Should().Be(createRecipeDto.Description);
        resultDto.Ingredients.Should().Be(createRecipeDto.Ingredients);
        resultDto.Instructions.Should().Be(createRecipeDto.Instructions);
        resultDto.PrepTimeMinutes.Should().Be(createRecipeDto.PrepTimeMinutes);
        resultDto.Category.Should().Be(createRecipeDto.Category);
        resultDto.ImageUrl.Should().Be(createRecipeDto.ImageUrl);

        // Verifica se os métodos mockados foram chamados como esperado
        _mockRecipeRepository.Verify(r => r.CreateAsync(It.Is<Recipe>(p => p.Name == createRecipeDto.Name)), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidDto_ShouldThrowValidationException()
    {
        // Arrange
        var createRecipeDto = new CreateRecipeRequestDto { Name = "", Description = "", Ingredients = "", Instructions = "", PrepTimeMinutes = -5, Category = "", ImageUrl = "" };
        var validationErrors = new List<ValidationFailure>
            {
                new ValidationFailure("Name", "O nome da receita é obrigatório."),
                new ValidationFailure("Description", "A descrição da receita é obrigatória."),
                new ValidationFailure("Ingredients", "Os ingredientes da receita é obrigatório."),
                new ValidationFailure("Instructions", "O modo de preparo da receita é obrigatório."),
                new ValidationFailure("PrepTimeMinutes", "O tempo de preparo da receita deve ser maior que zero."),
                new ValidationFailure("Category", "A categoria da receita é obrigatória."),
                new ValidationFailure("ImageUrl", "A imagem da receita é obrigatória."),
            };
        var validationResult = new ValidationResult(validationErrors);

        _mockCreateRecipeValidator
            .Setup(v => v.ValidateAsync(It.IsAny<CreateRecipeRequestDto>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(validationResult); // Resultado de validação inválido

        // Act & Assert
        // Verifica se uma ValidationException é lançada
        await _recipeService.Invoking(s => s.CreateAsync(createRecipeDto))
            .Should().ThrowAsync<ValidationException>()
            .WithMessage("*O nome da receita é obrigatório.*")
            .Where(ex => ex.Errors.Count() == 7);

        // Verifica que AddAsync e SaveChangesAsync não foram chamados
        _mockRecipeRepository.Verify(r => r.CreateAsync(It.IsAny<Recipe>()), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.Never);
    }

    #endregion

    #region Tests for GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_WithValidIdAndRecipeExists_ShouldReturnRecipeDto()
    {
        // Arrange
        var recipeId = Guid.NewGuid();

        var createRecipeDto = new CreateRecipeRequestDto { Name = "Receita Teste", Description = "Descrição Teste", Ingredients = "Tomate", Instructions = "Fatiar", PrepTimeMinutes = 5, Category = "Salada", ImageUrl = "url.com.br" };
        var recipe = new Recipe(createRecipeDto.Name, createRecipeDto.Description, createRecipeDto.Ingredients, createRecipeDto.Instructions, createRecipeDto.PrepTimeMinutes, createRecipeDto.Category, createRecipeDto.ImageUrl);
        // Para garantir que o ID é o mesmo, precisaríamos de um construtor que aceite o ID ou usar reflection,
        // mas para o teste do serviço, podemos assumir que o repositório retorna a entidade correta.
        // O mapeamento DTO <-> Entidade é o foco aqui.

        _mockRecipeRepository.Setup(r => r.GetByIdAsync(recipeId)).ReturnsAsync(recipe);

        // Act
        var resultDto = await _recipeService.GetByIdAsync(recipeId);

        // Assert
        resultDto.Should().NotBeNull();
        resultDto.Name.Should().Be(recipe.Name);
        resultDto.Description.Should().Be(recipe.Description);
        resultDto.PrepTimeMinutes.Should().Be(recipe.PrepTimeMinutes);

        _mockRecipeRepository.Verify(r => r.GetByIdAsync(recipeId), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentId_ShouldReturnNull()
    {
        // Arrange
        var produtoId = Guid.NewGuid();
        _mockRecipeRepository.Setup(r => r.GetByIdAsync(produtoId)).ReturnsAsync((Recipe)null); // Retorna nulo

        // Act
        var resultadoDto = await _recipeService.GetByIdAsync(produtoId);

        // Assert
        resultadoDto.Should().BeNull();
    }

    #endregion

    #region Tests for GetAllAsync

    [Fact]
    public async Task GetAllAsync_WhenRecipesExists_ShouldReturnReciptDtoColllection()
    {
        // Arrange
        var recipes = new List<Recipe>
            {
                new("Receita 1", "Descrição Teste", "Tomate", "Fatiar", 5, "Salada", "url.com.br"),
                new("Receita 2", "Descrição Teste", "Tomate", "Fatiar", 5, "Salada", "url.com.br")
            };
        _mockRecipeRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(recipes);

        // Act
        var result = await _recipeService.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.First().Name.Should().Be("Receita 1");
    }

    [Fact]
    public async Task GetAllAsync_WhenRecipesNonExists_ShouldReturnEmptyCollection()
    {
        // Arrange
        _mockRecipeRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Recipe>());

        // Act
        var result = await _recipeService.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    #endregion

    #region Tests for UpdateAsync

    [Fact]
    public async Task UpdateAsync_WithExistentRecipeAndValidDto_ShouldReturnTrue()
    {
        // Arrange
        var recipeId = Guid.NewGuid();
        var recipeExists = new Recipe("Receita 1", "Descrição Teste", "Tomate", "Fatiar", 5, "Salada", "url.com.br");
        var updateDto = new UpdateRecipeRequestDto { Name = "Nome Novo", Description = "Descrição Nova", Ingredients = "Tomate", Instructions = "Fatiar", PrepTimeMinutes = 5, Category = "Salada", ImageUrl = "url.com.br" };

        _mockUpdateRecipeValidator.Setup(v => v.ValidateAsync(updateDto, It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync(new ValidationResult());
        _mockRecipeRepository.Setup(r => r.GetByIdAsync(recipeId)).ReturnsAsync(recipeExists);
        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _recipeService.UpdateAsync(recipeId, updateDto);

        // Assert
        result.Should().BeTrue();
        recipeExists.Name.Should().Be(updateDto.Name); // Verifica se o método de atualização foi chamado na entidade

        _mockRecipeRepository.Verify(r => r.GetByIdAsync(recipeId), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistentRecipe_ShouldReturnFalse()
    {
        // Arrange
        var recipeId = Guid.NewGuid();
        var updateDto = new UpdateRecipeRequestDto { Name = "Nome Novo", Description = "Desc Nova", Ingredients = "Tomate", Instructions = "Fatiar", PrepTimeMinutes = 5, Category = "Salada", ImageUrl = "url.com.br" };

        _mockUpdateRecipeValidator.Setup(v => v.ValidateAsync(updateDto, It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync(new ValidationResult());
        _mockRecipeRepository.Setup(r => r.GetByIdAsync(recipeId)).ReturnsAsync((Recipe)null);

        // Act
        var result = await _recipeService.UpdateAsync(recipeId, updateDto);

        // Assert
        result.Should().BeFalse();
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.Never);
    }

    #endregion

    #region Tests for DeleteAsync

    [Fact]
    public async Task DeleteAsync_WithExistentRecipe_ShouldReturnTrue()
    {
        // Arrange
        var recipeId = Guid.NewGuid();
        var recipeExists = new Recipe("Receita 1", "Descrição Teste", "Tomate", "Fatiar", 5, "Salada", "url.com.br");

        _mockRecipeRepository.Setup(r => r.GetByIdAsync(recipeId)).ReturnsAsync(recipeExists);
        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _recipeService.DeleteAsync(recipeId);

        // Assert
        result.Should().BeTrue();
        _mockRecipeRepository.Verify(r => r.GetByIdAsync(recipeId), Times.Once);
        _mockRecipeRepository.Verify(r => r.DeleteAsync(recipeExists), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentRecipe_ShouldReturnFalse()
    {
        // Arrange
        var recipeId = Guid.NewGuid();
        _mockRecipeRepository.Setup(r => r.GetByIdAsync(recipeId)).ReturnsAsync((Recipe)null);

        // Act
        var result = await _recipeService.DeleteAsync(recipeId);

        // Assert
        result.Should().BeFalse();
        _mockRecipeRepository.Verify(r => r.DeleteAsync(It.IsAny<Recipe>()), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.Never);
    }

    #endregion
}