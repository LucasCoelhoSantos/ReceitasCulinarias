using System.ComponentModel.DataAnnotations;

namespace ReceitasCulinarias.Application.Auth.DTOs;

public class RegisterRequestDto
{
    [Required(ErrorMessage = "O nome de usuário é obrigatório.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "O nome de usuário deve ter entre 3 e 50 caracteres.")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "O email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Formato de email inválido.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "A senha e a confirmação de senha não correspondem.")]
    public string ConfirmPassword { get; set; }
}
