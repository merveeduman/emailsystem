using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EmailSystem.Api.Models;
using Microsoft.IdentityModel.Tokens;

namespace EmailSystem.Api.Services;

public class TokenService(IConfiguration configuration)
{
    public string CreateToken(User user)
    {
        var key = configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key missing");
        var issuer = configuration["Jwt:Issuer"] ?? "EmailSystem";
        var audience = configuration["Jwt:Audience"] ?? "EmailSystemClients";

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username)
        };

        var creds = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddHours(12),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

