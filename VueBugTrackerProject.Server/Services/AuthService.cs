using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VueBugTrackerProject.Classes;

namespace VueBugTrackerProject.Server
{
    /// <summary>
    /// A service to authenticate a user and their JWE tokens.
    /// </summary>
    public class AuthService
    {

        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Creates a JWT for a user.
        /// </summary>
        /// <param name="account">The account the token is being generated for.</param>
        /// <returns>A JWT.</returns>
        public string GenerateToken(Account account)
        {
            var handler = new JwtSecurityTokenHandler();

            var key = Encoding.UTF8.GetBytes(_configuration["JWTPrivateKey"]);

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha512Signature);
            

            var tokenDescriptor = new SecurityTokenDescriptor{
                Issuer = "Sample",
                Audience = "Sample",
                Subject = GetClaims(account),
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = signingCredentials,
            };

            var token = handler.CreateToken(tokenDescriptor);

            return handler.WriteToken(token);
        }

        /// <summary>
        /// Validates a user's token.
        /// </summary>
        /// <param name="token">The token to be validated.</param>
        /// <returns>True if the token is valid, false otherwise.</returns>
        public async Task<bool> ValidateToken(string token)
        {
           
            var handler = new JsonWebTokenHandler();

            TokenValidationResult result = await handler.ValidateTokenAsync(
                token,
            new TokenValidationParameters
            {
                ClockSkew = TimeSpan.Zero,
                ValidateLifetime = true,
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidIssuer = "Sample",
                ValidAudience = "Sample",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTPrivateKey"]))
            });

            return result.IsValid;
        }

        /// <summary>
        /// Genarates claims for a user.
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public ClaimsIdentity GetClaims(Account account)
        {
            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim("name", account.Username));
            claims.AddClaim(new Claim("email", account.Email));
            claims.AddClaim(new Claim("role", account.Role.ToString()));

            return claims;
        }
    }
}
