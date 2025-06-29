using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using NameSpace.Models;
using System.IdentityModel.Tokens.Jwt;

namespace NameSpace.Services
{
    public class TokenProvider(IConfiguration configuration)
    {
        public string Create(User user)
        {
            string secretKey = configuration["Jwt:Secret"]!;
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    [
                        new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                       // new Claim(JwtRegisteredClaimNames.Email, user.Email),
                       // new Claim("email_verified", user.EmailConfirmed.ToString()),
                    ]),
                Expires = DateTime.UtcNow.AddMinutes(configuration.GetValue<int>("Jwt:ExpirationInMinutes")),
                SigningCredentials = credentials,
                Issuer = configuration["Jwt:Issuer"],
                Audience = configuration["Jwt:Audience"]
            };

            var handler = new JsonWebTokenHandler(); 

            string token = handler.CreateToken(tokenDescriptor);

            return token;
        }
        public ClaimsPrincipal ValidateJwtToken(string token)
        {
            try
            {
                string? secretKey = configuration["Jwt:Secret"];

                if (string.IsNullOrWhiteSpace(secretKey))
                {
                    throw new InvalidOperationException("JWT secret key is missing from configuration!");
                }
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

                var tokenHandler = new JwtSecurityTokenHandler();

                // Definiera valideringsparametrarna
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = securityKey,
                    ClockSkew = TimeSpan.Zero // Justera om du vill tillåta en viss tidsmarginal
                };

                // Validera token
                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                return principal;
            }
            catch (Exception ex)
            {
                // Om valideringen misslyckas, returnera null eller kasta ett undantag
                throw new Exception("Failed to validate JWT token with exception: ", ex);
            }
        }
    }
}
