using Microsoft.AspNetCore.Identity;
using ReceitasCulinarias.Application.Autenticacao.DTOs;

namespace ReceitasCulinarias.Application.Autenticacao.Interfaces;

public interface IAuthService
{
    Task<IdentityResult> RegisterUserAsync(RegisterRequestDto registroDto);
    Task<LoginResponseDto?> LoginUserAsync(LoginRequestDto loginDto);
}
