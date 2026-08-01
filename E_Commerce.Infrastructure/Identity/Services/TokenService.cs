using E_Commerce.Application.Contracts;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Identity.Services
{
    internal class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;

        public TokenService(IOptions<JwtSettings> jwtOptions)
        {
            _jwtSettings = jwtOptions.Value;
        }
        public string CreateToken(string userId, string email, string userName, string displayName, IReadOnlyList<string> roles)
        {
            

            // Claims 

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Name, userName)
                //new Claim("DisplayName", displayName)
            };


            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            //signingCredentials(Secret Key, SecurityAlgorithm)

            var secKey = _jwtSettings.SecretKey;
            if(string.IsNullOrEmpty(secKey))
            {
                throw new InvalidOperationException("JWT SecretKey Is Missing");
            }

            if(secKey.Length < 32)
            {
                throw new InvalidOperationException("JWT SecretKey Is Too Short");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            // issuer + audeience + expires



            var token = new JwtSecurityToken
            (
                issuer: _jwtSettings.Issuer, // Create token issuer,
                audience: _jwtSettings.Audience, // Create token audience,
                claims: claims, // Create token claims based on userId, email, userName, displayName, and roles,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationInMinutes), // Set token expiration time,
                signingCredentials: credentials // Set signing credentials for the token
            );

            return  new JwtSecurityTokenHandler().WriteToken(token);

        }
    }

    public class JwtSettings
    {
        public string SecretKey { get; set; } = default!;
        public string Issuer { get; set; } = default!;
        public string Audience { get; set; } = default!;
        public int AccessTokenExpirationInMinutes { get; set; } 
    }
}
