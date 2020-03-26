using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
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
        private readonly IMapper _mapper;

        public UserManager(IAppContext appContext, IMapper mapper)
        {
            _appContext = appContext;
            _mapper = mapper;
        }

        public Task<UserFullDTO> GetUser(string userId)
        {
            //организованные, посещённые события и фото вручную
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

        public Task ChangeRole(int userId, int roleId)
        {
            throw new NotImplementedException();
        }
    }
}
