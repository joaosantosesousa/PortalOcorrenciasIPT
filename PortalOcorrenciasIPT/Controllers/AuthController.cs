using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PortalOcorrenciasIPT.Data;
using PortalOcorrenciasIPT.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PortalOcorrenciasIPT.Controllers;

// Controller responsável pela autenticação na API.
// Permite validar credenciais e devolver um token JWT ao cliente.
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    // Endpoint de login da API.
    // Recebe email e password e, se forem válidos, devolve um token JWT.
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Procura o utilizador pelo email recebido no DTO.
        ApplicationUser? user = await _userManager.FindByEmailAsync(dto.Email);

        if (user == null)
        {
            return Unauthorized(new
            {
                Mensagem = "Credenciais inválidas."
            });
        }

        // Valida a password usando o sistema de passwords do ASP.NET Identity.
        bool passwordValida = await _userManager.CheckPasswordAsync(user, dto.Password);

        if (!passwordValida)
        {
            return Unauthorized(new
            {
                Mensagem = "Credenciais inválidas."
            });
        }

        // Obtém as roles do utilizador para as incluir no token.
        // Isto permite proteger endpoints da API com base na role, por exemplo Gestor.
        IList<string> roles = await _userManager.GetRolesAsync(user);

        string token = CriarToken(user, roles);

        return Ok(new
        {
            Token = token,
            Email = user.Email,
            Roles = roles
        });
    }

    private string CriarToken(ApplicationUser user, IList<string> roles)
    {
        // Claims são informações incluídas no token JWT.
        // Aqui são guardados o identificador, email e nome do utilizador.
        List<Claim> claims = new()
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.Email ?? "")
        };

        // Cada role do utilizador é adicionada ao token.
        // Isto permite usar [Authorize(Roles = "Gestor")] nos endpoints da API.
        foreach (string role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Chave usada para assinar o token.
        // Em produção, esta chave é configurada fora do código, nas variáveis do Azure.
        SymmetricSecurityKey key = new(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

        SigningCredentials credentials = new(
            key,
            SecurityAlgorithms.HmacSha256);

        // Criação do token JWT com emissor, audiência, claims, tempo de validade e assinatura.
        JwtSecurityToken token = new(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(2),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}