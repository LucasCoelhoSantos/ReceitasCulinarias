using Microsoft.EntityFrameworkCore;
using ReceitasCulinarias.Domain.Entities;

namespace ReceitasCulinarias.Infrastructure.Data;
public class DataSeeder
{
    private readonly AppDbContext _context;

    public DataSeeder(AppDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        await _context.Database.MigrateAsync();

        if (await _context.Recipes.AnyAsync())
        {
            return;
        }

        var recipes = new List<Recipe>
            {
                new Recipe(
                    name: "Bolo de Chocolate Fofinho",
                    description: "Um bolo de chocolate clássico, perfeito para qualquer ocasião.",
                    ingredients: "Farinha, Açúcar, Chocolate em Pó, Ovos, Leite, Fermento",
                    instructions: "Misture os secos, adicione os molhados, asse por 40 min.",
                    prepTimeMinutes: 60,
                    category: "Sobremesa",
                    imageUrl: "https://placehold.co/300x200/d97706/white?text=Bolo+Chocolate"
                ),
                new Recipe(
                    name: "Salada Caesar Simples",
                    description: "Uma salada refrescante e saborosa.",
                    ingredients: "Alface Americana, Frango Grelhado, Croutons, Molho Caesar",
                    instructions: "Monte a salada e sirva com o molho.",
                    prepTimeMinutes: 20,
                    category: "Salada",
                    imageUrl: "https://placehold.co/300x200/10b981/white?text=Salada+Caesar"
                ),
                new Recipe(
                    name: "Lasanha à Bolonhesa",
                    description: "Uma lasanha rica e reconfortante.",
                    ingredients: "Massa de Lasanha, Molho Bolonhesa, Molho Branco, Queijo Mussarela",
                    instructions: "Monte as camadas e asse até dourar.",
                    prepTimeMinutes: 90,
                    category: "Prato Principal",
                    imageUrl: "https://placehold.co/300x200/ef4444/white?text=Lasanha"
                )
            };

        await _context.Recipes.AddRangeAsync(recipes);
        await _context.SaveChangesAsync();
    }
}
