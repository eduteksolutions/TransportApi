using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TransportApi.Services
{
    public class JwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public (string Token, DateTime Expiry) GenerateToken(
            int userId)
        {
            var expiry = DateTime.UtcNow.AddHours(1);

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"]!
                ));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);


            var claims = new[]
            {
            new Claim("UserID", userId.ToString())
        };


            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiry,
                signingCredentials: credentials
            );


            return (
                new JwtSecurityTokenHandler()
                .WriteToken(token),
                expiry
            );
        }
    }
}
