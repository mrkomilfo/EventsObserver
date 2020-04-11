using CSharpFunctionalExtensions;
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
        Task<Maybe<UserFullDTO>> GetUser(Guid userId, string hostRoot);
        Task<Page<UserLiteDTO>> GetUsers(int index, int pageSize);
        Task<bool> RegisterUser(RegisterDTO user);
        Task UpdateUser(UserUpdateDTO user, string hostRoot);
        Task<Maybe<UserToUpdateDTO>> GetUserToUpdate(Guid userId, string hostRoot);
        Task DeleteUser(Guid userId, bool force, string hostRoot);
        Task BanUser(BanDTO banDTO);
        Task UnbanUser(Guid userId);
        DateTime? GetUnlockTime(Guid userId);
        Task<Maybe<LoginResponseDTO>> Login(LoginDTO model);
        Task ChangeRole(ChangeRoleDTO changeRoleDTO);
        Task<IEnumerable<Role>> GetRoles();
        Task<Maybe<UserRoleDTO>> GetUserWithRole(Guid userId);
        Task<Role> GetUserRole(Guid userId);
    }
}
