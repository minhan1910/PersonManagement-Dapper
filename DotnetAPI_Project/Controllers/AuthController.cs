using DotnetAPI_Project.Data;
using DotnetAPI_Project.Dtos;
using DotnetAPI_Project.Extensions;
using DotnetAPI_Project.Helpers;
using DotnetAPI_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DotnetAPI_Project.Controllers
{
    [Authorize]    
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DataContextDapper _dapper;
        private readonly AuthHelper _authHelper;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
            _dapper = new DataContextDapper(configuration);
            _authHelper = new AuthHelper(configuration);
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register(UserForRegistrationDto userForRegistration)
        {
            if (userForRegistration.Password == userForRegistration.PasswordConfirm)
            {
                string sqlCheckUserExists = $"SELECT [Email] FROM TutorialAppSchema.Auth WHERE Email = '{userForRegistration.Email}'";

                IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExists);

                if (!existingUsers.Any())
                {
                    // password salt is random number generator
                    byte[] passwordSalt = new byte[128 / 8];
                    using (RandomNumberGenerator rng = RandomNumberGenerator.Create()) 
                    {
                        rng.GetNonZeroBytes(passwordSalt);
                    }

                    byte[] passwordHash = _authHelper.GetPasswordHash(userForRegistration.Password, passwordSalt);
                   
                    string sqlAddAuth = $@"
                        INSERT INTO TutorialAppSchema.Auth ([Email], [PasswordHash], [PasswordSalt])
                        values ('{userForRegistration.Email}', @PasswordHash, @PasswordSalt)";

                    List<SqlParameter> sqlParameters = new();

                    SqlParameter passwordHashParameter = new SqlParameter("@PasswordHash", SqlDbType.VarBinary);
                    passwordHashParameter.Value = passwordHash;

                    SqlParameter passwordSaltParameter = new SqlParameter("@PasswordSalt", SqlDbType.VarBinary);
                    passwordSaltParameter.Value = passwordSalt;

                    sqlParameters.Add(passwordHashParameter);
                    sqlParameters.Add(passwordSaltParameter);

                    if (_dapper.ExecuteSqlWithParamaters(sqlAddAuth, sqlParameters.ToArray()))
                    {
                        // for registration user details
                        string sqlToAddUser = $@"
                          insert into TutorialAppSchema.Users
                          (  
                              [FirstName]
                              ,[LastName]
                              ,[Email]
                              ,[Gender]
                              ,[Active]
                          ) values (
                               '{StringHelper.EscapeQuoteString(userForRegistration.FirstName)}'
                              ,'{StringHelper.EscapeQuoteString(userForRegistration.LastName)}'
                              ,'{StringHelper.EscapeQuoteString(userForRegistration.Email)}'
                              ,'{userForRegistration.Gender}'
                              ,1)";

                        if (_dapper.ExecuteSql(sqlToAddUser))
                        {
                            return Ok();
                        }
                        throw new Exception("Failed to add user");
                    }

                    throw new Exception("Failed to register user!");
                }

                throw new Exception("User with this email already exists!");
            }

            throw new Exception("Password do not match!");
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(UserForLoginDto userForLogin)
        {
            string sqlForHashAndSalt = $@"
                SELECT
                      [PasswordHash]
                      ,[PasswordSalt]
                FROM [TutorialAppSchema].[Auth]
                WHERE [Email] = '{userForLogin.Email}'
            ";

            UserForLoginConfirmationDto userForLoginConfirmation = 
                _dapper.LoadDataSingle<UserForLoginConfirmationDto>(sqlForHashAndSalt);

            byte[] passwordHash = _authHelper.GetPasswordHash(userForLogin.Password, userForLoginConfirmation.PasswordSalt);
            
            // can not compare by using passwordHash == userForLoginConfirmation.PasswordHash // becasue object -> won't work
            // loop through each array but i am using equal Span to speed up alogo - can convert it to extensions method
            if (passwordHash.ComparedTo(userForLoginConfirmation.PasswordHash) == false)
            {
                return StatusCode(401, "Incorrect password!");
            }

            string userIdSql = $"select UserId from TutorialAppSchema.Users where Email = '{userForLogin.Email}'";
            int userId = _dapper.LoadDataSingle<int>(userIdSql);

            return Ok(new Dictionary<string, string>
            {
                ["token"] = _authHelper.CreateToken(userId)
            });
        }

        // thorugh Authorize attribute
        [HttpGet("RefreshToken")]
        public string RefreshToken()
        {
            string userIdSql = $"SELECT UserId FROM TutorialAppSchema.Users WHERE UserId = '{User.FindFirst("userId")?.Value}'";

            int userId = _dapper.LoadDataSingle<int>(userIdSql);

            return _authHelper.CreateToken(userId);
        }


    }
}
