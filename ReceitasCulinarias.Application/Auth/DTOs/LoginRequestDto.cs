using System.ComponentModel.DataAnnotations;

namespace ReceitasCulinarias.Application.Autenticacao.DTOs;

public class LoginRequestDto
{
    [Required(ErrorMessage = "O email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Formato de email inválido.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}
