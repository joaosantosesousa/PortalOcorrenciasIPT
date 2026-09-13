using System.ComponentModel.DataAnnotations;

namespace PortalOcorrenciasIPT.DTOs;

public class LoginDto
{
    [Required(ErrorMessage = "O email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Email inválido.")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "A password é obrigatória.")]
    public string Password { get; set; } = "";
}