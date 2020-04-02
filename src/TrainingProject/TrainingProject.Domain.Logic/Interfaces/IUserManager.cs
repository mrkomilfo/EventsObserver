using CSharpFunctionalExtensions;
using System;
using System.Threading.Tasks;
using TrainingProject.DomainLogic.Models.Common;
using TrainingProject.DomainLogic.Models.Users;

namespace TrainingProject.DomainLogic.Interfaces
{
    public interface IUserManager
    {
        Task<Maybe<UserFullDTO>> GetUser(Guid userId, string hostRoot);
        Task<Page<UserLiteDTO>> GetUsers(int index, int pageSize);
        Task<bool> RegisterUser(RegisterDTO user);
        Task<Maybe<LoginResponseDTO>> Login(LoginDTO user);
        Task UpdateUser(UserUpdateDTO user, string hostRoot);
        Task DeleteUser(Guid userId, bool force, string hostRoot);
        Task BanUser(BanDTO banDTO);
        Task UnbanUser(Guid userId);
        Task ChangeRole(Guid userId, int roleId);
    }
}
