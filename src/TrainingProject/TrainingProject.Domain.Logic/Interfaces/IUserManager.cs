using CSharpFunctionalExtensions;
using System.Threading.Tasks;
using TrainingProject.DomainLogic.Models.Common;
using TrainingProject.DomainLogic.Models.Users;

namespace TrainingProject.DomainLogic.Interfaces
{
    public interface IUserManager
    {
        Task<Maybe<UserFullDTO>> GetUser(int userId, string hostRoot);
        Task<Page<UserLiteDTO>> GetUsers(int index, int pageSize);
        Task<bool> RegisterUser(RegisterDTO user);
        Task<Maybe<LoginResponseDTO>> Login(LoginDTO user);
        Task UpdateUser(UserUpdateDTO user);
        Task DeleteUser(int userId, bool force);
        Task BanUser(int userId, int? days, int? hours);
        Task UnbanUser(int userId);
        Task ChangeRole(int userId, int roleId);
    }
}
