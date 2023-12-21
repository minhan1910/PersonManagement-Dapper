
using DotnetAPI_Project.Models;

namespace DotnetAPI_Project.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContextEF _entityFramework;

        public UserRepository(IConfiguration configuration)
        {
            _entityFramework = new DataContextEF(configuration);
        }

        public bool SaveChanges()
        {
            return _entityFramework.SaveChanges() > 0;
        }

        public bool AddEntity<T>(T entityToAdd)
        {
            if (entityToAdd is not null)
            {
                _entityFramework.Add(entityToAdd);
                return true;
            }
            return false;
        }
        public void RemoveEntity<T>(T entityToRemove)
        {
            if (entityToRemove is not null)
            {
                _entityFramework.Remove(entityToRemove);
            }
        }

        public IEnumerable<User> GetUsers()
        {
            return _entityFramework.Users.ToList();
        }

        public User GetSingleUser(int userId)
        {
            User? user = _entityFramework.Users.FirstOrDefault(user => user.UserId == userId);

            if (user != null)
            {
                return user;
            }

            throw new Exception("Failed to Get User");
        }

        public UserSalary GetUserSalary(int userId)
        {
            UserSalary? userSalary = _entityFramework.UserSalary
             .Where(u => u.UserId == userId)
             .FirstOrDefault<UserSalary>();

            if (userSalary != null)
            {
                return userSalary;
            }

            throw new Exception("Failed to Get User");
        }

        public UserJobInfo GetJobInfo(int userId)
        {
            UserJobInfo? userJobInfo = _entityFramework.UserJobInfo
               .Where(u => u.UserId == userId)
               .FirstOrDefault<UserJobInfo>();

            if (userJobInfo != null)
            {
                return userJobInfo;
            }

            throw new Exception("Failed to Get User");
        }
    }
}
