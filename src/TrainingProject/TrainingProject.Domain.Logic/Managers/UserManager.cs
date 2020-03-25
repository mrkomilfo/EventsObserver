using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TrainingProject.Data;
using TrainingProject.Domain;
using TrainingProject.DomainLogic.Interfaces;
using TrainingProject.DomainLogic.Models.Common;
using TrainingProject.DomainLogic.Models.Users;

namespace TrainingProject.DomainLogic.Managers
{
    public class UserManager : IUserManager
    {
        private readonly IAppContext _appContext;

        public UserManager(IAppContext appContext)
        {
            _appContext = appContext;
        }

        public Task<UserFullDTO> GetUser(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<Page<UserLiteDTO>> GetUsers(int index, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RegisterUser(RegisterDTO user)
        {
            throw new NotImplementedException();
        }

        public Task Login(LoginDTO user)
        {
            throw new NotImplementedException();
        }

        public Task UpdateUser(UserUpdateDTO user)
        {
            throw new NotImplementedException();
        }

        public Task DeleteUser(int userId, bool force)
        {
            throw new NotImplementedException();
        }

        public Task BanUser(int userId, int days)
        {
            throw new NotImplementedException();
        }
    }
}
