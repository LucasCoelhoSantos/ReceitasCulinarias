using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ReceitasCulinarias.API.Middleware;
using ReceitasCulinarias.Application.Auth.Interfaces;
using ReceitasCulinarias.Application.Auth.Services;
using ReceitasCulinarias.Application.Recipes.Interfaces;
using ReceitasCulinarias.Application.Recipes.Services;
using ReceitasCulinarias.Application.Recipes.Validators;
using ReceitasCulinarias.Domain.Entities;
using ReceitasCulinarias.Domain.Interfaces;
using ReceitasCulinarias.Infrastructure.Data;
using ReceitasCulinarias.Infrastructure.Repositories;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost", "http://nginx-proxy")
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});

// Configurar String de Conex�o e DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString,
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);

            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null
            );
        }
    ));

// Configurar ASP.NET Core Identity
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    // Configura��es de senha
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;

    // Configura��es de Lockout
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // Configura��es de Usu�rio
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders(); // Adiciona provedores de token padr�o (para reset de senha, etc.)

// Configurar Autentica��o JWT
var jwtKey = builder.Configuration["JwtSettings:Key"];
var jwtIssuer = builder.Configuration["JwtSettings:Issuer"];
var jwtAudience = builder.Configuration["JwtSettings:Audience"];

if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
{
    throw new ArgumentNullException("Configura��es de JWT (Key, Issuer, Audience) n�o encontradas ou inv�lidas no appsettings.json");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = builder.Environment.IsProduction();
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

// Registrar Reposit�rios e outros Servi�os
builder.Services.AddScoped<IRecipeRepository, RecipeRepository>();
builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());
builder.Services.AddScoped<IRecipeService, RecipeService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Registrar todos os validadores do assembly que cont�m CreateRecipeRequestValidator (Desafio.Application)
builder.Services.AddValidatorsFromAssemblyContaining<CreateRecipeRequestValidator>(); ;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Desafio API - Cat�logo Interativo de Receitas Culin�rias", Version = "v1" });

    // Define o esquema de seguran�a Bearer (JWT)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Por favor, insira 'Bearer' seguido de um espa�o e o token JWT",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

try
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.MigrateAsync();

        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Migrations aplicadas com sucesso na inicializa��o.");
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Ocorreu um erro durante a aplica��o das migrations.");
}

// Configure the HTTP request pipeline.
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection(); // Remover esse middleware se o Docker estiver configurado apenas para http
app.UseRouting();
app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
