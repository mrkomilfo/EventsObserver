using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
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

        public async Task<bool> RegisterUser(RegisterDTO user)
        {
            if (await _appContext.Users.AnyAsync(u => u.Login == user.Login))
            {
                return false;
            }
            User newUser = _mapper.Map<User>(user);
            newUser.RoleId = (await _appContext.Roles.FirstOrDefaultAsync(r => r.Name == "User"))?.Id;
            await _appContext.Users.AddAsync(newUser);
            await _appContext.SaveChangesAsync(default);
            return true;
        }

        public Task Login(LoginDTO user)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateUser(UserUpdateDTO user)
        {
            User updatedUser = await _appContext.Users.FindAsync(user.Id);
            if (updatedUser != null)
            {
                _mapper.Map(user, updatedUser);
            }
            await _appContext.SaveChangesAsync(default);
        }

        public async Task DeleteUser(int userId, bool force)
        {
            var user = await _appContext.Users.IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null)
            {
                if (force)
                {
                    _appContext.Users.Remove(user);
                }
                else
                {
                    user.IsDeleted = true;
                }
                await _appContext.SaveChangesAsync(default);
            }
        }

        public async Task BanUser(int userId, int? days, int? hours)
        {
            var user = await _appContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null)
            {
                user.UnlockTime = DateTime.Now.AddDays(days ?? 0);
                user.UnlockTime = DateTime.Now.AddHours(hours ?? 0);
            }
            await _appContext.SaveChangesAsync(default);
        }

        public async Task UnbanUser(int userId)
        {
            var user = await _appContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null)
            {
                user.UnlockTime = DateTime.Now;
            }
            await _appContext.SaveChangesAsync(default);
        }

        public async Task ChangeRole(int userId, int roleId)
        {
            var user = await _appContext.Users.FindAsync(userId);
            if (user != null)
            {
                user.RoleId = roleId;
            }
            await _appContext.SaveChangesAsync(default);
        }
    }
}
