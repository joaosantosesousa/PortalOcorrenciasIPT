using System.ComponentModel.DataAnnotations;

namespace PortalOcorrenciasIPT.DTOs;

// DTO usado no endpoint de login da API.
// Recebe as credenciais necessárias para autenticar o utilizador e gerar o token JWT.
public class LoginDto
{
    [Required(ErrorMessage = "O email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Email inválido.")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "A password é obrigatória.")]
    public string Password { get; set; } = "";
}