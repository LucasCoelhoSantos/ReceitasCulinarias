namespace ReceitasCulinarias.Domain.Entities;

public class Recipe
{
    public Guid Id { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public DateTime UpdatedDate { get; private set; }
    public string Name { get; private set; } // Nome da Receita
    public string Description { get; private set; } // Breve descrição
    public string Ingredients { get; private set; } // Lista de ingredientes (pode ser um JSON string ou texto simples)
    public string Instructions { get; private set; } // Modo de preparo
    public int PrepTimeMinutes { get; private set; } // Tempo de preparo em minutos
    public string Category { get; private set; } // Ex: Sobremesa, Prato Principal, Salada
    public string ImageUrl { get; private set; } // URL para uma imagem da receita

    public Recipe() { }

    public Recipe(string name, string description, string ingredients, string instructions, int prepTimeMinutes, string category, string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("O nome da receita não pode ser vazio.", nameof(name));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("A descrição da receita não pode ser vazia.", nameof(description));
        if (string.IsNullOrWhiteSpace(ingredients))
            throw new ArgumentException("Os ingredientes da receita não pode ser vazio.", nameof(ingredients));
        if (string.IsNullOrWhiteSpace(instructions))
            throw new ArgumentException("O modo de preparo da receita não pode ser vazia.", nameof(instructions));
        if (prepTimeMinutes <= 0)
            throw new ArgumentException("O tempo de preparo em minutos da receita deve ser maior que zero.", nameof(prepTimeMinutes));
        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("A categoria da receita não pode ser vazia.", nameof(category));
        if (string.IsNullOrWhiteSpace(imageUrl))
            throw new ArgumentException("A imagem da receita não pode ser vazia.", nameof(imageUrl));

        Id = Guid.NewGuid();
        CreatedDate = DateTime.UtcNow;
        UpdatedDate = DateTime.UtcNow;
        Name = name;
        Description = description;
        Ingredients = ingredients;
        Instructions = instructions;
        PrepTimeMinutes = prepTimeMinutes;
        Category = category;
        ImageUrl = imageUrl;
    }

    public void AtualizarDetalhes(string name, string description, string ingredients, string instructions, int prepTimeMinutes, string category, string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("O nome da receita não pode ser vazio.", nameof(name));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("A descrição da receita não pode ser vazia.", nameof(description));
        if (string.IsNullOrWhiteSpace(ingredients))
            throw new ArgumentException("Os ingredientes da receita não pode ser vazio.", nameof(ingredients));
        if (string.IsNullOrWhiteSpace(instructions))
            throw new ArgumentException("O modo de preparo da receita não pode ser vazia.", nameof(instructions));
        if (prepTimeMinutes <= 0)
            throw new ArgumentException("O tempo de preparo em minutos da receita deve ser maior que zero.", nameof(prepTimeMinutes));
        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("A categoria da receita não pode ser vazia.", nameof(category));
        if (string.IsNullOrWhiteSpace(imageUrl))
            throw new ArgumentException("A imagem da receita não pode ser vazia.", nameof(imageUrl));

        UpdatedDate = DateTime.UtcNow;
        Name = name;
        Description = description;
        Ingredients = ingredients;
        Instructions = instructions;
        PrepTimeMinutes = prepTimeMinutes;
        Category = category;
        ImageUrl = imageUrl;
    }
}
