using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace CityGuide.Services
{
    public class TokenService
    {
        private readonly string _secret = ConfigurationManager.AppSettings["JwtSecret"];
        private readonly string _issuer = ConfigurationManager.AppSettings["JwtIssuer"];
        private readonly string _audience = ConfigurationManager.AppSettings["JwtAudience"];

        public string GenerateToken(int userId)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]{new Claim(ClaimTypes.NameIdentifier, userId.ToString())};

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            try
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));

                var parameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = _issuer,
                    ValidAudience = _audience,
                    IssuerSigningKey = key,

                    ClockSkew = TimeSpan.Zero
                };

                var handler = new JwtSecurityTokenHandler();

                return handler.ValidateToken(token, parameters, out _);
            }
            catch (SecurityTokenException)
            {
                return null;
            }
            catch (ArgumentException)
            {
                return null;
            }
        }
    }
}