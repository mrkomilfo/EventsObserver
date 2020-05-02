using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrainingProject.Domain;
using TrainingProject.DomainLogic.Models.Common;
using TrainingProject.DomainLogic.Models.Users;

namespace TrainingProject.DomainLogic.Interfaces
{
    public interface IUserManager
    {
        Task<UserFullDTO> GetUser(Guid userId);
        Task<Page<UserLiteDTO>> GetUsers(int index, int pageSize);
        Task RegisterUser(RegisterDTO user);
        Task UpdateUser(UserUpdateDTO user, string hostRoot);
        Task<UserToUpdateDTO> GetUserToUpdate(Guid userId);
        Task DeleteUser(Guid userId, bool force, string hostRoot);
        Task BanUser(BanDTO banDTO);
        Task UnbanUser(Guid userId);
        Task<DateTime?> GetUnlockTime(Guid userId);
        Task<LoginResponseDTO> Login(LoginDTO model);
        Task ChangeRole(ChangeRoleDTO changeRoleDTO);
        Task<IEnumerable<Role>> GetRoles();
        Task<UserRoleDTO> GetUserWithRole(Guid userId);
        Task<Role> GetUserRole(Guid userId);
        Task ChangePassword(ChangePasswordDTO changePasswordDTO);
    }
}
