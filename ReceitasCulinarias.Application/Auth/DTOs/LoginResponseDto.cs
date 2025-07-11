﻿namespace ReceitasCulinarias.Application.Auth.DTOs;

public class LoginResponseDto
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Token { get; set; }
    public DateTime Expiration { get; set; }
    public IEnumerable<string> Roles { get; set; }
}
