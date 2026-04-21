using Entities.Models;
using Logic.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Logic.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(Organizer organizer)
        {
            var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key is not configured.");

            var jwtIssuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT issuer is not configured.");

            var jwtAudience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT audience is not configured.");

            var expiryMinutes = int.TryParse(_configuration["Jwt:AccessTokenMinutes"], out var parsedMinutes) ? parsedMinutes : 60;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, organizer.Id),
                new Claim(ClaimTypes.Email, organizer.Email),
                new Claim(ClaimTypes.Name, organizer.Name)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddMinutes(expiryMinutes);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public DateTime GetExpiryUtc()
        {
            var expiryMinutes = int.TryParse(_configuration["Jwt:AccessTokenMinutes"], out var parsedMinutes) ? parsedMinutes : 60;

            return DateTime.UtcNow.AddMinutes(expiryMinutes);
        }
    }
}
