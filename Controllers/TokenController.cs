using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using TransportApi.Models;

namespace TransportApi.Controllers
{
    
    
    [Route("api/[controller]")]
    [ApiController]
   
        public class TokenController : ControllerBase
        {
            private readonly IConfiguration _configuration;

            public TokenController(IConfiguration configuration)
            {
                _configuration = configuration;
            }

            [HttpPost("generate")]
            public IActionResult Generate([FromBody] GenerateTokenRequest request)
            {
                if (request == null || request.UserId <= 0)
                {
                    return BadRequest(new
                    {
                        Code = 400,
                        Status = false,
                        Message = "Invalid UserID"
                    });
                }

                try
                {
                    var issuer = _configuration["Jwt:Issuer"];
                    var audience = _configuration["Jwt:Audience"];
                    var secretKey = _configuration["Jwt:Key"];

                    var expirationDate = DateTime.UtcNow.AddHours(1);

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

                    var credentials = new SigningCredentials(
                        key,
                        SecurityAlgorithms.HmacSha256);

                    var claims = new[]
                    {
                    new Claim("UserID", request.UserId.ToString())
                };

                    var token = new JwtSecurityToken(
                        issuer: issuer,
                        audience: audience,
                        claims: claims,
                        expires: expirationDate,
                        signingCredentials: credentials);

                    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                    string connectionString = _configuration.GetConnectionString("DefaultConnection");

                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();

                        string query = @"INSERT INTO UserTokens
                                    (UserID, Token, CreatedOn, ExpiresOn)
                                     VALUES
                                    (@UserID,@Token,@CreatedOn,@ExpiresOn)";

                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@UserID", request.UserId);
                            cmd.Parameters.AddWithValue("@Token", tokenString);
                            cmd.Parameters.AddWithValue("@CreatedOn", DateTime.UtcNow);
                            cmd.Parameters.AddWithValue("@ExpiresOn", expirationDate);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    return Ok(new
                    {
                        Code = 200,
                        Status = true,
                        Token = tokenString,
                        ExpiresOn = expirationDate
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        Code = 500,
                        Status = false,
                        Message = ex.Message
                    });
                }
            }

            private string GenerateRandomBase64Key()
            {
                byte[] keyBytes = new byte[32];
                RandomNumberGenerator.Fill(keyBytes);
                return Convert.ToBase64String(keyBytes);
            }
        }
    

}


