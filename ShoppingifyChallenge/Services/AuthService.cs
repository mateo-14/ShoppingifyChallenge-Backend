using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShoppingifyChallenge.Data;
using ShoppingifyChallenge.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ShoppingifyChallenge.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly ShoppingListContext _dbContext;
        public AuthService(ShoppingListContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<string> GenerateMagiclinkToken(string email)
        {
            var user = await _dbContext.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
            if (user == null)
            {
                user = new User { Email = email, AuthProvider = AuthProvider.MagicLink };
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();
            }

            var token = GenerateMagiclinkJWT(user.Id);
            // TODO Send email with magic link
            return token;
        }

        public async Task<Result<string>> LoginWithMagiclinkToken(string token)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("MagiclinkJWT:Secret")));
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey,
                ValidateIssuer = false,
                ValidateAudience = false
            };
            var handler = new JwtSecurityTokenHandler();
            var result = await handler.ValidateTokenAsync(token, validationParameters);

            if (!result.IsValid)
            {
                return Result.Fail("Invalid token");
            }
          
            if (await IsMagiclinkTokenInBlacklist(token))
            {
                return Result.Fail("Token is in blacklist");
            }

            _dbContext.MagiclinkTokenBlacklist.Add(new MagiclinkToken { Token = token });
            await _dbContext.SaveChangesAsync();

            var nameIdentifier = result.Claims.First(c => c.Key == ClaimTypes.NameIdentifier).Value.ToString();
            var parseResult = int.TryParse(nameIdentifier, out var id);
            if (!parseResult)
            {
                return Result.Fail("Invalid token");
            }

            var user = await _dbContext.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
            if (user == null)
            {
                return Result.Fail("Invalid token");
            }

            var jwt = GenerateAuthJWT(user.Id);
            return Result.Ok(jwt);
        }

        /// <summary>
        /// Generate JWT used in the magic link. This JWT is used to authenticate the user when he clicks on the magic link.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private string GenerateMagiclinkJWT(int userId)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("MagiclinkJWT:Secret")));

            var token = new JwtSecurityToken(
                claims: new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim("uuid", Guid.NewGuid().ToString())
                },
                expires: DateTime.Now.AddHours(1),
                signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Generate JWT used to authorize the user when using the API.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private string GenerateAuthJWT(int userId)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("Jwt:Secret")));

            var token = new JwtSecurityToken(
                claims: new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                },
                issuer: _configuration.GetValue<string>("Jwt:Issuer"),
                audience: _configuration.GetValue<string>("Jwt:Audience"),
                expires: DateTime.Now.AddHours(1),
                signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public Task<bool> IsMagiclinkTokenInBlacklist(string token)
        {
            return _dbContext.MagiclinkTokenBlacklist.AnyAsync(t => t.Token == token);
        }
    }
}
