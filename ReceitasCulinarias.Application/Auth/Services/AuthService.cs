using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using ReceitasCulinarias.Application.Auth.DTOs;
using ReceitasCulinarias.Application.Auth.Interfaces;
using ReceitasCulinarias.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ReceitasCulinarias.Application.Auth.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;
    private readonly IValidator<RegisterRequestDto> _registerValidator;
    private readonly IValidator<LoginRequestDto> _loginValidator;

    public AuthService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        IConfiguration configuration,
        ILogger<AuthService> logger,
        IValidator<RegisterRequestDto> registerValidator,
        IValidator<LoginRequestDto> loginValidator)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _logger = logger;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
    }

    public async Task<IdentityResult> RegisterUserAsync(RegisterRequestDto registroDto)
    {
        var validationResult = await _registerValidator.ValidateAsync(registroDto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => new IdentityError { Code = e.ErrorCode, Description = e.ErrorMessage });
            return IdentityResult.Failed(errors.ToArray());
        }

        var userExists = await _userManager.FindByEmailAsync(registroDto.Email);
        if (userExists != null)
        {
            _logger.LogWarning("Tentativa de registro com email já existente: {Email}", registroDto.Email);
            return IdentityResult.Failed(new IdentityError { Code = "EmailInUse", Description = "Este email já está em uso." });
        }

        userExists = await _userManager.FindByNameAsync(registroDto.UserName);
        if (userExists != null)
        {
            _logger.LogWarning("Tentativa de registro com nome de usuário já existente: {UserName}", registroDto.UserName);
            return IdentityResult.Failed(new IdentityError { Code = "UserNameInUse", Description = "Este nome de usuário já está em uso." });
        }

        var user = new AppUser
        {
            UserName = registroDto.UserName,
            Email = registroDto.Email,
            SecurityStamp = Guid.NewGuid().ToString() // SecurityStamp é importante
        };

        var result = await _userManager.CreateAsync(user, registroDto.Password);

        if (result.Succeeded)
        {
            _logger.LogInformation("Usuário {UserName} registrado com sucesso.", user.UserName);
            // Opcional: Adicionar a um role padrão. Ex: await _userManager.AddToRoleAsync(user, "UsuarioComum");
        }
        else
        {
            _logger.LogWarning("Falha ao registrar usuário {UserName}. Erros: {Errors}", user.UserName, result.Errors.Select(e => e.Description));
        }
        return result;
    }

    public async Task<LoginResponseDto?> LoginUserAsync(LoginRequestDto loginDto)
    {
        var validationResult = await _loginValidator.ValidateAsync(loginDto);
        if (!validationResult.IsValid)
        {
            // Poderia lançar ValidationException ou retornar null/uma resposta de erro específica.
            // Por simplicidade, retornaremos null indicando falha de validação inicial.
            _logger.LogWarning("Validação de login falhou para o email: {Email}. Erros: {Errors}",
               loginDto.Email, validationResult.Errors.Select(e => e.ErrorMessage));
            return null; // Ou um DTO de erro
        }

        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null)
        {
            _logger.LogWarning("Tentativa de login para email não registrado: {Email}", loginDto.Email);
            return null; // Usuário não encontrado
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            _logger.LogWarning("Tentativa de login falhou para o usuário: {Email}. Motivo: {SignInResult}", loginDto.Email, result.ToString());
            // Não especificar se é usuário ou senha incorreta por segurança.
            return null; // Credenciais inválidas ou conta bloqueada
        }

        var userRoles = await _userManager.GetRolesAsync(user);
        var tokenDescriptor = GenerateJwtTokenDescriptor(user, userRoles);
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        _logger.LogInformation("Usuário {Email} logado com sucesso.", user.Email);

        return new LoginResponseDto
        {
            UserId = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Token = tokenString,
            Expiration = token.ValidTo,
            Roles = userRoles
        };
    }

    private SecurityTokenDescriptor GenerateJwtTokenDescriptor(AppUser user, IList<string> roles)
    {
        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id), // ID do usuário
                new Claim(JwtRegisteredClaimNames.Sub, user.Email), // 'Subject' - geralmente o email ou username
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT ID único
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64) // 'Issued At'
            };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]
            ?? throw new InvalidOperationException("Chave JWT não configurada.")));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        var expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:DurationInMinutes"] ?? "60"));

        return new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expires,
            Issuer = _configuration["JwtSettings:Issuer"] ?? throw new InvalidOperationException("Emissor JWT não configurado."),
            Audience = _configuration["JwtSettings:Audience"] ?? throw new InvalidOperationException("Audiência JWT não configurada."),
            SigningCredentials = creds
        };
    }
}
