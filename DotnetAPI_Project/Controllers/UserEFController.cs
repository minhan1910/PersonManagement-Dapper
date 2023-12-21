using AutoMapper;
using DotnetAPI_Project.Data;
using DotnetAPI_Project.Dtos;
using DotnetAPI_Project.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI_Project.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserEFController : ControllerBase
    {
        private readonly DataContextEF _entityFramework;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserEFController(IConfiguration configuration, IUserRepository userRepository)
        {
            _entityFramework = new DataContextEF(configuration);
            _mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserToAddDto, User>();
                cfg.CreateMap<User, User>();
                cfg.CreateMap<UserSalaryToEditDto, UserSalary>();
                cfg.CreateMap<UserSalaryToAddDto, UserSalary>();
                cfg.CreateMap<UserJobInfo, UserJobInfo>();
            }));
            _userRepository = userRepository;
        }

        #region Users

        [HttpGet("[action]")]
        public IEnumerable<User> GetUsers()
        {
            return _userRepository.GetUsers();
        }

        [HttpGet("[action]/{userId}")]
        public User GetSingleUser(int userId)
        {
            User? user = _userRepository.GetSingleUser(userId);

            if (user == null)
            {
                throw new Exception("User is not exist");
            }

            return user;
        }

        [HttpPut("EditUser")]
        public IActionResult EditUser(User user)
        {
            User? userFromDb = _userRepository.GetSingleUser(user.UserId);

            if (userFromDb is not null)
            {
                _mapper.Map(user, userFromDb);

                if (_userRepository.SaveChanges())
                {
                    return Ok();
                }

                throw new Exception("Fail to update user");
            }

            throw new Exception("Fail to get user");
        }

        [HttpPost("AddUser")]
        public IActionResult AddUser(UserToAddDto userToAddDto)
        {
            //User user = new User();
            //user.FirstName = userToAddDto.FirstName ;
            //user.LastName = userToAddDto.LastName ;
            //user.Gender = userToAddDto.Gender ;
            //user.Email = userToAddDto.Email ;
            //user.Active = userToAddDto.Active;
            User user = _mapper.Map<User>(userToAddDto);

            //_entityFramework.Users.Add(user);
            _userRepository.AddEntity<User>(user);  

            if (_userRepository.SaveChanges() )
            {
                return Ok();
            }

            throw new Exception("Fail to add new user");
        }

        [HttpDelete("[action]/{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            User? userFromDb = _userRepository.GetSingleUser(userId);

            if (userFromDb is not null)
            {
                //_entityFramework.Users.Remove(userFromDb);
                _userRepository.RemoveEntity(userFromDb);

                if (_userRepository.SaveChanges() )
                {
                    return Ok();
                }
                throw new Exception("Fail to delete user with id = " + userId);
            }

            throw new Exception("Fail to get user with id = " + userId);
        }


        #endregion

        #region UserSalary

        [HttpGet("[action]")]
        public IEnumerable<UserSalary> GetUserSalaries()
        {
            return _entityFramework.UserSalary.ToList();
        }

        [HttpGet("[action]/{userId}")]
        public UserSalary GetUserSalary(int userId)
        {
            UserSalary? userSalary = _userRepository.GetUserSalary(userId);

            if (userSalary is not null)
            {
                return userSalary;
            }

            throw new Exception("Get User failed");
        }

        [HttpPut("[action]")]
        public IActionResult EditUserSalary(UserSalaryToEditDto userSalaryToEdit)
        {
            UserSalary? userSalary = _userRepository.GetUserSalary(userSalaryToEdit.UserId);

            if (userSalary is not null)
            {
                _mapper.Map(userSalaryToEdit, userSalary);

                if (_userRepository.SaveChanges() )
                {
                    return Ok();
                }
                throw new Exception("Updating UserSalary failed on save");
            }
            throw new Exception("Get User failed");
        }

        [HttpPost("[action]")]
        public IActionResult AddUserSalary(UserSalaryToAddDto userSalaryToAdd)
        {
            UserSalary userSalary = _mapper.Map<UserSalary>(userSalaryToAdd);

            //_entityFramework.UserSalary.Add(userSalary);
            _userRepository.RemoveEntity(userSalary);

            if (_userRepository.SaveChanges() )
            {
                return Ok(userSalary);
            }

            throw new Exception("Adding UserSalary failed on save");
        }

        [HttpDelete("[action]/{userId}")]
        public IActionResult DeleteUserSalary(int userId)
        {
            UserSalary? userSalary = _userRepository.GetUserSalary(userId);

            if (userSalary is not null)
            {
                //_entityFramework.UserSalary.Remove(userSalary);
                _userRepository.RemoveEntity(userSalary);

                if (_userRepository.SaveChanges() )
                {
                    return Ok();
                }
                throw new Exception("Deleting UserSalary failed on save");
            }
            throw new Exception("Get User failed");

        }

        #endregion

        #region UserJobInfo

        [HttpGet("UserJobInfo/{userId}")]
        public UserJobInfo GetUserJobInfoEF(int userId)
        {
            return _userRepository.GetJobInfo(userId);
        }

        [HttpPost("UserJobInfo")]
        public IActionResult PostUserJobInfoEf(UserJobInfo userForInsert)
        {
            _userRepository.AddEntity(userForInsert);
            if (_userRepository.SaveChanges() )
            {
                return Ok();
            }
            throw new Exception("Adding UserJobInfo failed on save");
        }


        [HttpPut("UserJobInfo")]
        public IActionResult PutUserJobInfoEf(UserJobInfo userForUpdate)
        {
            UserJobInfo? userToUpdate = _userRepository.GetJobInfo(userForUpdate.UserId);

            if (userToUpdate != null)
            {
                _mapper.Map(userForUpdate, userToUpdate);
                if (_userRepository.SaveChanges() )
                {
                    return Ok();
                }
                throw new Exception("Updating UserJobInfo failed on save");
            }
            throw new Exception("Failed to find UserJobInfo to Update");
        }


        [HttpDelete("UserJobInfo/{userId}")]
        public IActionResult DeleteUserJobInfoEf(int userId)
        {
            UserJobInfo? userToDelete = _userRepository.GetJobInfo(userId);

            if (userToDelete != null)
            {
                _userRepository.RemoveEntity(userToDelete);
                if (_userRepository.SaveChanges() )
                {
                    return Ok();
                }
                throw new Exception("Deleting UserJobInfo failed on save");
            }
            throw new Exception("Failed to find UserJobInfo to delete");
        }

        #endregion
    }
}

