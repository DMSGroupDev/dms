using dms_backend_api.Domain.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace dms_backend_api.Services.Identity
{
    public partial class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(ApplicationUser user, List<string> applicationRoles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var authClaims = new Dictionary<string, object>
            {
                { ClaimTypes.Name, user.UserName },{JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString() },
            };

            foreach (var role in applicationRoles)
                authClaims.Add(ClaimTypes.Role, role);

            var JWTSecret = (string)_configuration.GetValue(typeof(string), "JWTSecret");
            if (string.IsNullOrEmpty(JWTSecret))
                throw new InvalidOperationException("The JWT Secret is empty.");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = (string)_configuration.GetValue(typeof(string), "JWT_ValidIssuer"),
                Audience = (string)_configuration.GetValue(typeof(string), "JWT_ValidAudience"),
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), new Claim(ClaimTypes.Email, user.Email) }),
                Expires = DateTime.UtcNow.AddHours(2),
                Claims = authClaims,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(JWTSecret)), SecurityAlgorithms.HmacSha256Signature),
                IssuedAt = DateTime.UtcNow
            };
            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }

        /* public int? ValidateToken(string token)
         {
             if (token == null)
                 return null;

             var tokenHandler = new JwtSecurityTokenHandler();
             var key = Encoding.ASCII.GetBytes((string)_configuration.GetValue(typeof(string), "Secret"));
             try
             {
                 tokenHandler.ValidateToken(token, new TokenValidationParameters
                 {
                     ValidateIssuerSigningKey = true,
                     IssuerSigningKey = new SymmetricSecurityKey(key),
                     ValidateIssuer = false,
                     ValidateAudience = false,
                     // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                     ClockSkew = TimeSpan.Zero
                 }, out SecurityToken validatedToken);

                 var jwtToken = (JwtSecurityToken)validatedToken;
                 var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

                 // return user id from JWT token if validation successful
                 return userId;
             }
             catch
             {
                 // return null if validation fails
                 return null;
             }
         }*/
    }
}
