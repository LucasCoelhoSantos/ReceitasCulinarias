namespace ReceitasCulinarias.Application.Recipes.DTOs;

public class RecipeDto
{
    public Guid Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public string Name { get; set; } // Nome da Receita
    public string Description { get; set; } // Breve descrição
    public string Ingredients { get; set; } // Lista de ingredientes (pode ser um JSON string ou texto simples)
    public string Instructions { get; set; } // Modo de preparo
    public int PrepTimeMinutes { get; set; } // Tempo de preparo em minutos
    public string Category { get; set; } // Ex: Sobremesa, Prato Principal, Salada
    public string ImageUrl { get; set; } // URL para uma imagem da receita
}
