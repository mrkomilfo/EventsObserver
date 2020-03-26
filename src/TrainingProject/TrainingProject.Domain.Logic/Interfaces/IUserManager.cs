using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TrainingProject.Domain;
using TrainingProject.DomainLogic.Models.Common;
using TrainingProject.DomainLogic.Models.Users;

namespace TrainingProject.DomainLogic.Interfaces
{
    public interface IUserManager
    {
        Task<UserFullDTO> GetUser(string userId);
        Task<Page<UserLiteDTO>> GetUsers(int index, int pageSize);
        Task<bool> RegisterUser(RegisterDTO user);
        Task Login(LoginDTO user);
        Task UpdateUser(UserUpdateDTO user);
        Task DeleteUser(int userId, bool force);
        Task BanUser(int userId, int days);
        Task ChangeRole(int userId, int roleId);
    }
}
