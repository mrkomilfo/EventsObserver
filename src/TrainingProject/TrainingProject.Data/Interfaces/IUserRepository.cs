using System.Threading.Tasks;
using TrainingProject.Domain;

namespace TrainingProject.Data.Interfaces
{
    interface IUserRepository
    {
        Task<User> GetUser(string login);
        Task<bool> IsUserExist(string login);
        Task AddUser(User user);
        Task UpdateUser(User user);
        Task DeleteUser(int id);
    }
}
