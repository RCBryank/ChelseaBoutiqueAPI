using Chelsea_Boutique.Models;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Chelsea_Boutique.Services
{
    public class TokenProvider(IConfiguration configuration)
    {
        public string Create(WebUser user)
        {
            string secretkey = configuration["Jwt:Secret"]!;
            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretkey));

            var credentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("sub", user.ID.ToString()),
                    new Claim("email", user.Email)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims, "custom"),
                Expires = DateTime.UtcNow.AddMinutes(configuration.GetValue<int>("Jwt:ExpirationInMinutes")),
                NotBefore = DateTime.UtcNow,
                SigningCredentials = credentials,
                Issuer = configuration["Jwt:Issuer"],
                Audience = configuration["Jwt:Audience"]
            };

            var handler = new JwtSecurityTokenHandler();

            var token = handler.CreateToken(tokenDescriptor);
            string tokentostring = handler.WriteToken(token);

            return tokentostring;
        }

    }
}
