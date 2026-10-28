using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ClyvoCareOSChallenge.Controllers;

public record LoginRequest(string Usuario, string Senha);

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var senhaEsperada = _configuration["DemoAuth:Password"];

        if (string.IsNullOrWhiteSpace(senhaEsperada))
            return StatusCode(503, new { mensagem = "Login não configurado." });

        var usuarioValido = request.Usuario == "admin";

        var esperado = SHA256.HashData(
            Encoding.UTF8.GetBytes(senhaEsperada));

        var recebido = SHA256.HashData(
            Encoding.UTF8.GetBytes(request.Senha ?? ""));

        var senhaValida = CryptographicOperations.FixedTimeEquals(
            esperado, recebido);

        if (!usuarioValido || !senhaValida)
            return Unauthorized(new { mensagem = "Credenciais inválidas." });

        var key = _configuration["Jwt:Key"];
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];

        if (string.IsNullOrWhiteSpace(key) ||
            string.IsNullOrWhiteSpace(issuer) ||
            string.IsNullOrWhiteSpace(audience))
            return StatusCode(503, new { mensagem = "JWT não configurado." });

        Claim[] claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "admin"),
            new Claim(ClaimTypes.Name, "admin"),
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        DateTime expires = DateTime.UtcNow.AddMinutes(60);

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Convert.FromBase64String(key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials);

        return Ok(new
        {
            access_token = new JwtSecurityTokenHandler().WriteToken(token),
            token_type = "Bearer",
            expires_at = expires
        });
    }

    [Authorize(
        AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
        Roles = "Admin")]
    [HttpGet("validar")]
    public IActionResult Validar()
    {
        return Ok(new
        {
            mensagem = "JWT válido. Acesso autorizado.",
            usuario = User.Identity?.Name
        });
    }
}
