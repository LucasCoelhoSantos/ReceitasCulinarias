using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReceitasCulinarias.Domain.Entities;
using ReceitasCulinarias.Domain.Interfaces;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;


namespace ReceitasCulinarias.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<AppUser, IdentityRole, string>, IUnitOfWork
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<Recipe> Recipes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
