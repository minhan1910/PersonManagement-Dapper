using DotnetAPI_Project.Constants;
using DotnetAPI_Project.Data;
using DotnetAPI_Project.Dtos;
using DotnetAPI_Project.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI_Project.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]

    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DataContextDapper _dapper;

        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
            _dapper = new DataContextDapper(configuration);
        }

        [HttpGet("[action]")]
        public DateTime TestConnection()
        {
            return _dapper.LoadDataSingle<DateTime>("select getdate()");
        }

        #region Users

        [HttpGet]
        public IEnumerable<User> GetUsers()
        {
            return _dapper.LoadData<User>("Select * from TutorialAppSchema.Users");
        }

        [HttpGet("{userId}")]
        public User GetSingleUser(int userId)
        {
            return _dapper.LoadDataSingle<User>("Select * from TutorialAppSchema.Users where UserId = " + userId);
        }

        [HttpPut]
        public IActionResult EditUser(User user)
        {
            string sql = $@"
                  update TutorialAppSchema.Users
                  set [FirstName] = '{EscapeQuoteString(user.FirstName)}'
                      ,[LastName] = '{EscapeQuoteString(user.LastName)}'
                      ,[Email] = '{EscapeQuoteString(user.Email)}'
                      ,[Gender] = '{user.Gender}'
                      ,[Active] = '{user.Active}'
                  where UserId = '{user.UserId}'
            ";

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Fail to update user");
        }

        private string EscapeQuoteString(string str) => str.Replace("'", "''");

        [HttpPost]
        public IActionResult AddUser(UserToAddDto user)
        {
            string sql = $@"
                  insert into TutorialAppSchema.Users
                  (  
                      [FirstName]
                      ,[LastName]
                      ,[Email]
                      ,[Gender]
                      ,[Active]
                  ) values (
                       '{EscapeQuoteString(user.FirstName)}'
                      ,'{EscapeQuoteString(user.LastName)}'
                      ,'{EscapeQuoteString(user.Email)}'
                      ,'{user.Gender}'
                      ,'{user.Active}')
            ";

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Fail to add new user");
        }

        [HttpDelete("{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            string sql = @$"delete from TutorialAppSchema.Users where UserId = {userId}";

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Fail to delete user with id = " + userId);
        }

        #endregion

        #region UserSalary

        [HttpGet]
        public IEnumerable<UserSalary> GetUserSalaries()
        {
            string sql = $"select * from {DbSchemaConstants.DEFAULT_SCHEMA}.{nameof(UserSalary)}";
            return _dapper.LoadData<UserSalary>(sql);
        }

        [HttpGet("{userId}")]
        public UserSalary GetUserSalary(int userId)
        {
            string sql = $"select * from {DbSchemaConstants.DEFAULT_SCHEMA}.{nameof(UserSalary)} where UserId = {userId}";
            return _dapper.LoadDataSingle<UserSalary>(sql);
        }

        [HttpPut]
        public IActionResult EditUserSalary(UserSalaryToEditDto userSalaryToAddDto)
        {
            string sql = $@"
              update {DbSchemaConstants.DEFAULT_SCHEMA}.{nameof(UserSalary)}
              set [Salary] = '{userSalaryToAddDto.Salary}',
                  [AvgSalary] = '{userSalaryToAddDto.AvgSalary}'
              where UserId = {userSalaryToAddDto.UserId}
            ";

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Fail to update new user salary");
        }

        [HttpPost]
        public IActionResult AddUserSalary(UserSalaryToAddDto userSalaryToAddDto)
        {
            string sql = $@"
              insert into {DbSchemaConstants.DEFAULT_SCHEMA}.{nameof(UserSalary)} ([UserId], [Salary])
              values ('{userSalaryToAddDto.UserId}', '{userSalaryToAddDto.Salary}')              
            ";

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Fail to add new user salary");
        }

        [HttpDelete("{userId}")]
        public IActionResult DeleteUserSalary(int userId)
        {
            string sql = $@"delete from{DbSchemaConstants.DEFAULT_SCHEMA}.{nameof(UserSalary)} where UserId = {userId}";

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Fail to add new user salary");
        }

        #endregion

        #region UserJobInfo

        [HttpGet("UserJobInfo/{userId}")]
        public IEnumerable<UserJobInfo> GetUserJobInfo(int userId)
        {
            return _dapper.LoadData<UserJobInfo>(@"
            SELECT  UserJobInfo.UserId
                    , UserJobInfo.JobTitle
                    , UserJobInfo.Department
            FROM  TutorialAppSchema.UserJobInfo
                WHERE UserId = " + userId.ToString());
        }

        [HttpPost("UserJobInfo")]
        public IActionResult PostUserJobInfo(UserJobInfo userJobInfoForInsert)
        {
            string sql = @"
            INSERT INTO TutorialAppSchema.UserJobInfo (
                UserId,
                Department,
                JobTitle
            ) VALUES (" + userJobInfoForInsert.UserId
                    + ", '" + userJobInfoForInsert.Department
                    + "', '" + userJobInfoForInsert.JobTitle
                    + "')";

            if (_dapper.ExecuteSql(sql))
            {
                return Ok(userJobInfoForInsert);
            }
            throw new Exception("Adding User Job Info failed on save");
        }

        [HttpPut]
        public IActionResult PutUserJobInfo(UserJobInfo userJobInfoForUpdate)
        {
            string sql = "UPDATE TutorialAppSchema.UserJobInfo SET Department='"
                + userJobInfoForUpdate.Department
                + "', JobTitle='"
                + userJobInfoForUpdate.JobTitle
                + "' WHERE UserId=" + userJobInfoForUpdate.UserId.ToString();

            if (_dapper.ExecuteSql(sql))
            {
                return Ok(userJobInfoForUpdate);
            }
            throw new Exception("Updating User Job Info failed on save");
        }

        [HttpDelete("{userId}")]
        public IActionResult DeleteUserJobInfo(int userId)
        {
            string sql = @"
            DELETE FROM TutorialAppSchema.UserJobInfo 
                WHERE UserId = " + userId.ToString();

            Console.WriteLine(sql);

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to Delete User");
        }

        #endregion
    }
}

