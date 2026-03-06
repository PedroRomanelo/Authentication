using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EmpresaExemplo.Enums;
using Microsoft.IdentityModel.Tokens;


namespace EmpresaExemplo.Services;

public class TokenService
{
    private readonly IConfiguration _configuration; 

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(int userId, string email, States states)
    {
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]!);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
        {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim("state", states.ToString())
        }),
            Expires = DateTime.UtcNow.AddHours(24),//validade de 1 dia
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
