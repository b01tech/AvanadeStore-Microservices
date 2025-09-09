using Auth.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Auth.Application.Services.Token;
internal class TokenService : ITokenService
{
    private readonly IConfiguration _Configuration;

    public TokenService(IConfiguration configuration)
    {
        _Configuration = configuration;
    }

    public string GenereteAccessToken(User user)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtSecret = Encoding.UTF8.GetBytes(_Configuration["Jwt:Secret"]!);
        var key = new SymmetricSecurityKey(jwtSecret);
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            SigningCredentials = credentials,
            Expires = DateTime.UtcNow.AddMinutes(_Configuration.GetValue<double>("Jwt:DurationInMinutes")),
            Issuer = _Configuration["Jwt:Issuer"],
            Audience = _Configuration["Jwt:Audience"],
            Subject = GenerateClaims(user)
        };

        var token = handler.CreateToken(tokenDescriptor);

        return handler.WriteToken(token);
    }
    private static ClaimsIdentity GenerateClaims(User user)
    {
        var claims = new ClaimsIdentity();
        claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
        claims.AddClaim(new Claim(ClaimTypes.Role, user.Role.ToString()));

        return claims;
    }
}
