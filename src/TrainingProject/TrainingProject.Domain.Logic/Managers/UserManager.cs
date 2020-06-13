﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TrainingProject.Common;
using TrainingProject.Data;
using TrainingProject.Domain;
using TrainingProject.DomainLogic.Helpers;
using TrainingProject.DomainLogic.Interfaces;
using TrainingProject.DomainLogic.Models.Common;
using TrainingProject.DomainLogic.Models.Users;
using TrainingProject.Web.Helpers;

namespace TrainingProject.DomainLogic.Managers
{
    public class UserManager : IUserManager
    {
        private readonly IAppContext _appContext;
        private readonly IMapper _mapper;
        private readonly ILogHelper _logger;

        public UserManager(IAppContext appContext, IMapper mapper, ILogHelper logger)
        {
            _appContext = appContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<UserFullDTO> GetUser(Guid userId)
        {
            _logger.LogMethodCallingWithObject(new { userId });
            var DBUser = await _appContext.Users.Include(u => u.Role).Include(u => u.OrganizedEvents).FirstOrDefaultAsync(u => u.Id == userId);
            if (DBUser == null)
            {
                throw new KeyNotFoundException($"User with id={userId} not found");
            }
            var user = _mapper.Map<UserFullDTO>(DBUser);
            user.VisitedEvents = await _appContext.EventsUsers.Where(eu => eu.ParticipantId == userId).CountAsync();

            string imageName = DBUser.HasPhoto ? userId.ToString() : "default";
            string path = $"img\\users\\{imageName}.jpg";
            user.Photo = path;

            return user;
        }

        public async Task<Page<UserLiteDTO>> GetUsers(int index, int pageSize, string search)
        {
            _logger.LogMethodCallingWithObject(new { index, pageSize, search });
            var result = new Page<UserLiteDTO>() { CurrentPage = index, PageSize = pageSize };
            var query = _appContext.Users.Include(e => e.Role).AsQueryable();
            if (search != null)
            {
                query = query.Where(u => u.UserName.ToLower().Contains(search.ToLower()));
            }
            result.TotalRecords = await query.CountAsync();
            result.Records = await _mapper.ProjectTo<UserLiteDTO>(query).ToListAsync(default);
            return result;
        }

        public async Task RegisterUser(RegisterDTO user)
        {
            _logger.LogMethodCallingWithObject(user, "Password, PasswordConfirm");
            if (await _appContext.Users.AnyAsync(u => string.Equals(u.Login.ToLower(), user.Login.ToLower())))
            {
                throw new ArgumentException($"User with login \"{user.Login}\" already exist");
            }
            User newUser = _mapper.Map<User>(user);
            newUser.RoleId = (await _appContext.Roles.FirstOrDefaultAsync(r => r.Name == "User"))?.Id;
            await _appContext.Users.AddAsync(newUser);
            await _appContext.SaveChangesAsync(default);
        }

        public async Task UpdateUser(UserUpdateDTO user, string hostRoot)
        {
            _logger.LogMethodCallingWithObject(user);
            User updatedUser = await _appContext.Users.FirstOrDefaultAsync(u => Equals(u.Id, Guid.Parse(user.Id)));
            if (updatedUser == null)
            {
                throw new KeyNotFoundException($"User with id={user.Id} not found");
            }
            _mapper.Map(user, updatedUser);
            if (user.Photo != null)
            {
                string path = $"{hostRoot}\\wwwroot\\img\\users\\{updatedUser.Id}.jpg";
                _logger.LogInfo($"Saving image to {path}");
                await using var fileStream = new FileStream(path, FileMode.Create);
                await user.Photo.CopyToAsync(fileStream);
            }
            await _appContext.SaveChangesAsync(default);
        }

        public async Task<UserToUpdateDTO> GetUserToUpdate(Guid userId)
        {
            _logger.LogMethodCallingWithObject(new { userId });
            User user = await _appContext.Users.FindAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with id={user.Id} not found");
            }
            UserToUpdateDTO userToUpdate = _mapper.Map<UserToUpdateDTO>(user);
            if (userToUpdate.HasPhoto)
            {
                userToUpdate.Photo = $"img\\users\\{userId}.jpg";
            }
            return userToUpdate;
        }

        public async Task DeleteUser(Guid userId, bool force, string hostRoot)
        {
            _logger.LogMethodCallingWithObject(new { userId, force, hostRoot });
            var user = await _appContext.Users.IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with id={user.Id} not found");
            }
            if (force)
            {
                _appContext.Users.Remove(user);
                string path = $"{hostRoot}\\wwwroot\\img\\users\\{userId}.jpg";
                _logger.LogInfo($"Deleting image from {path}");
                File.Delete(path);
            }
            else
            {
                user.IsDeleted = true;
            }
            await _appContext.SaveChangesAsync(default);
        }

        public async Task<UserToBanDTO> GetUserToBan(Guid userId)
        {
            _logger.LogMethodCallingWithObject(new { userId });
            var user = await _appContext.Users.FirstOrDefaultAsync(u => Equals(u.Id, userId));
            if (user == null)
            {
                throw new KeyNotFoundException($"User with id={user.Id} not found");
            }
            UserToBanDTO userToBan = _mapper.Map<UserToBanDTO>(user);
            return userToBan;
        }

        public async Task BanUser(BanDTO banDTO)
        {
            _logger.LogMethodCallingWithObject(banDTO);
            var user = await _appContext.Users.FirstOrDefaultAsync(u => Equals(u.Id, Guid.Parse(banDTO.Id)));
            if (user == null)
            {
                throw new KeyNotFoundException($"User with id={user.Id} not found");
            }
            user.UnlockTime = DateTime.Now.AddDays(banDTO?.Days ?? 0);
            user.UnlockTime = user.UnlockTime?.AddHours(banDTO?.Hours ?? 0);
            await _appContext.SaveChangesAsync(default);
        }

        public async Task UnbanUser(Guid userId)
        {
            _logger.LogMethodCallingWithObject(new { userId });
            var user = await _appContext.Users.FirstOrDefaultAsync(u => Equals(u.Id, userId));
            if (user == null)
            {
                throw new KeyNotFoundException($"User with id={user.Id} not found");
            }
            user.UnlockTime = DateTime.Now;
            await _appContext.SaveChangesAsync(default);
        }

        public async Task ChangeRole(ChangeRoleDTO changeRoleDTO)
        {
            _logger.LogMethodCallingWithObject(changeRoleDTO);
            var user = await _appContext.Users.FindAsync(Guid.Parse(changeRoleDTO.UserId));
            if (user == null)
            {
                throw new KeyNotFoundException($"User with id={user.Id} not found");
            }
            if (!await _appContext.Roles.AnyAsync(r => r.Id == changeRoleDTO.RoleId))
            {
                throw new KeyNotFoundException($"Role with id={changeRoleDTO.RoleId} not found");
            }
            user.RoleId = changeRoleDTO.RoleId;
            await _appContext.SaveChangesAsync(default);
        }
        public async Task<DateTime?> GetUnlockTime(Guid userId)
        {
            _logger.LogMethodCallingWithObject(new { userId });
            if (!await _appContext.Users.AnyAsync(u => Equals(u.Id, userId)))
            {
                throw new KeyNotFoundException($"User with id={userId} not found");
            }
            return (await _appContext.Users.FirstAsync(u => Equals(u.Id, userId))).UnlockTime;
        }
        public async Task<LoginResponseDTO> Login(LoginDTO loginDto)
        {
            _logger.LogMethodCallingWithObject(loginDto, "Password");
            var identity = await GetIdentity(loginDto.Login, loginDto.Password);
            if (identity == null)
            {
                throw new UnauthorizedAccessException($"Wrong login or password");
            }
            var unlockTime = await GetUnlockTime(Guid.Parse(identity.Name));
            if (unlockTime != null && ((unlockTime ?? DateTime.Now) > DateTime.Now))
            {
                throw new UnauthorizedAccessException($"Banned until {unlockTime?.ToString("f")}");
            }

            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new LoginResponseDTO
            {
                AccessToken = encodedJwt,
                Name = identity.Name,
                Role = identity.Claims.Where(c => c.Type == ClaimTypes.Role).FirstOrDefault().Value,
            };
            return response;
        }
        private async Task<ClaimsIdentity> GetIdentity(string login, string password)
        {
            _logger.LogMethodCallingWithObject(new { login, password }, "password");
            ClaimsIdentity identity = null;
            var user = await _appContext.Users.Include(u => u.Role).FirstOrDefaultAsync(u => string.Equals(u.Login, login));
            if (user == null)
            {
                return null;
            }
            var passwordHash = HashGenerator.Encrypt(password);
            if (passwordHash == user.Password)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Id.ToString()),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.Name)
                };
                identity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            }
            return identity;
        }

        public async Task<IEnumerable<Role>> GetRoles()
        {
            _logger.LogMethodCalling();
            return await _appContext.Roles.ToListAsync();
        }

        public async Task<UserRoleDTO> GetUserWithRole(Guid userId)
        {
            _logger.LogMethodCallingWithObject(new { userId });
            User user = await _appContext.Users.FirstOrDefaultAsync(u => Equals(u.Id, userId));
            if (user == null)
            {
                throw new KeyNotFoundException($"User with id={user.Id} not found");
            }
            UserRoleDTO userRoleDTO = _mapper.Map<UserRoleDTO>(user);
            return userRoleDTO;
        }

        public async Task<Role> GetUserRole(Guid userId)
        {
            _logger.LogMethodCallingWithObject(new { userId });
            if (!await _appContext.Users.AnyAsync(u => Equals(u.Id, userId)))
            {
                throw new KeyNotFoundException($"Event with id={userId} not found");
            }
            return await _appContext.Users.Include(u => u.Role).Where(u => Equals(u.Id, userId)).Select(u => u.Role).FirstOrDefaultAsync();
        }

        public async Task ChangePassword(ChangePasswordDTO changePasswordDTO)
        {
            _logger.LogMethodCallingWithObject(changePasswordDTO, "OldPassword, NewPassword, NewPasswordConfirm");
            User user = await _appContext.Users.FirstOrDefaultAsync(u => Equals(u.Id, Guid.Parse(changePasswordDTO.Id)));
            if (user == null)
            {
                throw new KeyNotFoundException($"User with id={user.Id} not found");
            }
            if (!String.Equals(HashGenerator.Encrypt(changePasswordDTO.OldPassword), user.Password))
            {
                throw new ArgumentException("Wrong old password");
            }
            user.Password = HashGenerator.Encrypt(changePasswordDTO.NewPassword);
            await _appContext.SaveChangesAsync(default);
        }
    }
}
