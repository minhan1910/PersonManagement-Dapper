using DotnetAPI_Project.Data;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DotnetAPI_Project.Helpers
{
    public class AuthHelper
    {
        private readonly IConfiguration _configuration;

        public AuthHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public byte[] GetPasswordHash(string password, byte[] passwordSalt)
        {
            string passwordSaltPlusString = _configuration.GetSection("AppSettings")
                                                          .GetValue<string>("PasswordKey")!
                                            + Convert.ToBase64String(passwordSalt);

            byte[] passwordHash = KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 1000000,
                numBytesRequested: 256 / 8
            );
            return passwordHash;
        }

        public string CreateToken(int userId)
        {
            Claim[] claims = new Claim[]
            {
                new Claim("userId", userId.ToString())
            };

            string? tokenKeyString = _configuration.GetSection("AppSettings:TokenKey").Value;
            SymmetricSecurityKey tokenKey = new(Encoding.ASCII.GetBytes(tokenKeyString ?? string.Empty));
            SigningCredentials credentials = new(tokenKey, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = credentials,
                Expires = DateTime.Now.AddDays(1)
            };

            JwtSecurityTokenHandler tokenHandler = new();

            SecurityToken token = tokenHandler.CreateToken(securityTokenDescriptor);

            return tokenHandler.WriteToken(token);
        }


    }
}
