using Microsoft.AspNetCore.Identity;
using ReceitasCulinarias.Application.Auth.DTOs;

namespace ReceitasCulinarias.Application.Auth.Interfaces;

public interface IAuthService
{
    Task<IdentityResult> RegisterUserAsync(RegisterRequestDto registroDto);
    Task<LoginResponseDto?> LoginUserAsync(LoginRequestDto loginDto);
}
