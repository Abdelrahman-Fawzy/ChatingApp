using ChatingApp.BackEnd.Interfaces;
using ChatingApp.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChatingApp.BackEnd.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration config)
        {
            _configuration = config;
        }

        public string CreateToken(AppUser user)
        {
            var key = _configuration["JWT:Key"] ?? String.Empty;
            if (!string.IsNullOrEmpty(key) && key.Length < 64) throw new Exception("Your Key Is Invalid");

            var claims = new List<Claim>
            {
                new (ClaimTypes.Email, user.Email),
                new (ClaimTypes.NameIdentifier, user.DisplayName),
                new (System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, user.Id.ToString()),
            };

            var secureKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));

            var credentials = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = _configuration["JWT:Issuer"],
                Audience = _configuration["JWT:Audience"],
                SigningCredentials = new SigningCredentials(secureKey, SecurityAlgorithms.HmacSha512)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(credentials);

            return tokenHandler.WriteToken(token);
        }
    }
}
