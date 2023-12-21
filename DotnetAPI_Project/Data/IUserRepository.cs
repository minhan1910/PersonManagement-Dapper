using DotnetAPI_Project.Models;

namespace DotnetAPI_Project.Data
{
    public interface IUserRepository
    {
        bool SaveChanges();
        bool AddEntity<T>(T entityToAdd);
        void RemoveEntity<T>(T entityToRemove);
        IEnumerable<User> GetUsers();
        User GetSingleUser(int userId);
        UserSalary GetUserSalary(int userId);
        UserJobInfo GetJobInfo(int userId);
    }
}
