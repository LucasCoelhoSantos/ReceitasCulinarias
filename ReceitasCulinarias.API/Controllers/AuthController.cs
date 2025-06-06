using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReceitasCulinarias.API.Models;
using ReceitasCulinarias.Application.Autenticacao.DTOs;
using ReceitasCulinarias.Application.Autenticacao.Interfaces;

namespace ReceitasCulinarias.API.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto registroDto)
    {
        _logger.LogInformation("Tentativa de registro para o email {Email}", registroDto.Email);
        var result = await _authService.RegisterUserAsync(registroDto);

        if (result.Succeeded)
        {
            _logger.LogInformation("Usuário {Email} registrado com sucesso.", registroDto.Email);
            return Ok(new { Message = "Usuário registrado com sucesso." });
        }

        _logger.LogWarning("Falha no registro do usuário {Email}. Erros: {Errors}",
            registroDto.Email, result.Errors.Select(e => e.Description));

        // Retorna os erros do IdentityResult
        return BadRequest(result.Errors.Select(e => new { e.Code, e.Description }));
    }

    [HttpPost("login")]
    [AllowAnonymous] // Permite acesso anônimo a este endpoint
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)] // Para credenciais inválidas ou validação
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
    {
        _logger.LogInformation("Tentativa de login para o email {Email}", loginDto.Email);
        var loginResponse = await _authService.LoginUserAsync(loginDto);

        if (loginResponse != null)
        {
            _logger.LogInformation("Usuário {Email} logado com sucesso.", loginDto.Email);
            return Ok(loginResponse);
        }

        _logger.LogWarning("Falha no login para o email {Email}. Credenciais inválidas ou usuário não encontrado.", loginDto.Email);
        return BadRequest(new ErrorResponse(StatusCodes.Status400BadRequest, "Tentativa de login falhou. Verifique suas credenciais."));
    }
}
