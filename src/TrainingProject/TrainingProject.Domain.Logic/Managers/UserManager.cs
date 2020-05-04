using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

        public UserManager(IAppContext appContext, IMapper mapper)
        {
            _appContext = appContext;
            _mapper = mapper;
        }

        public async Task<UserFullDTO> GetUser(Guid userId)
        {
            var DBUser = await _appContext.Users.Include(u => u.Role).Include(u => u.OrganizedEvents).FirstOrDefaultAsync(u => u.Id == userId);
            if (DBUser == null)
            {
                throw new NullReferenceException($"User with id={userId} not found");
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
            User updatedUser = await _appContext.Users.FirstOrDefaultAsync(u => Guid.Equals(u.Id, Guid.Parse(user.Id)));
            if (updatedUser == null)
            {
                throw new NullReferenceException($"User with id={user.Id} not found");
            }
            _mapper.Map(user, updatedUser);
            if (user.Photo != null)
            {
                string path = $"{hostRoot}\\wwwroot\\img\\users\\{updatedUser.Id}.jpg";
                await using var fileStream = new FileStream(path, FileMode.Create);
                await user.Photo.CopyToAsync(fileStream);
            }
            await _appContext.SaveChangesAsync(default);
        }

        public async Task<UserToUpdateDTO> GetUserToUpdate(Guid userId)
        {
            User user = await _appContext.Users.FindAsync(userId);
            if (user == null)
            {
                throw new NullReferenceException($"User with id={user.Id} not found");
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
            var user = await _appContext.Users.IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new NullReferenceException($"User with id={user.Id} not found");
            }
            if (force)
            {
                _appContext.Users.Remove(user);
                string path = $"{hostRoot}\\wwwroot\\img\\users\\{userId}.jpg";
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
            var user = await _appContext.Users.FirstOrDefaultAsync(u => Guid.Equals(u.Id, userId));
            if (user == null)
            {
                throw new NullReferenceException($"User with id={user.Id} not found");
            }
            UserToBanDTO userToBan = _mapper.Map<UserToBanDTO>(user);
            return userToBan;
        }

        public async Task BanUser(BanDTO banDTO)
        {
            var user = await _appContext.Users.FirstOrDefaultAsync(u => Guid.Equals(u.Id, Guid.Parse(banDTO.Id)));
            if (user == null)
            {
                throw new NullReferenceException($"User with id={user.Id} not found");
            }
            user.UnlockTime = DateTime.Now.AddDays(banDTO?.Days ?? 0);
            user.UnlockTime = user.UnlockTime?.AddHours(banDTO?.Hours ?? 0);
            await _appContext.SaveChangesAsync(default);
        }

        public async Task UnbanUser(Guid userId)
        {
            var user = await _appContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new NullReferenceException($"User with id={user.Id} not found");
            }
            user.UnlockTime = DateTime.Now;
            await _appContext.SaveChangesAsync(default);
        }

        public async Task ChangeRole(ChangeRoleDTO changeRoleDTO)
        {
            var user = await _appContext.Users.FindAsync(Guid.Parse(changeRoleDTO.UserId));
            if (user == null)
            {
                throw new NullReferenceException($"User with id={user.Id} not found");
            }
            if (!await _appContext.Roles.AnyAsync(r => r.Id == changeRoleDTO.RoleId))
            {
                throw new NullReferenceException($"Role with id={changeRoleDTO.RoleId} not found");
            }
            user.RoleId = changeRoleDTO.RoleId;
            await _appContext.SaveChangesAsync(default);
        }
        public async Task<DateTime?> GetUnlockTime(Guid userId)
        {
            if (!await _appContext.Users.AnyAsync(u => Guid.Equals(u.Id, userId)))
            {
                throw new NullReferenceException($"User with id={userId} not found");
            }
            return (await _appContext.Users.FirstAsync(u => Guid.Equals(u.Id, userId))).UnlockTime;
        }
        public async Task<LoginResponseDTO> Login(LoginDTO model)
        {
            var identity = await GetIdentity(model.Login, model.Password);
            if (identity == null)
            {
                throw new NullReferenceException($"Wrong login or password");
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
            ClaimsIdentity identity = null;
            var user = await _appContext.Users.Include(u => u.Role).FirstOrDefaultAsync(u => string.Equals(u.Login, login));
            if (user == null)
            {
                throw new NullReferenceException($"User with id={user.Id} not found");
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
            return await _appContext.Roles.ToListAsync();
        }

        public async Task<UserRoleDTO> GetUserWithRole(Guid userId)
        {
            User user = await _appContext.Users.FirstOrDefaultAsync(u => Guid.Equals(u.Id, userId));
            if (user == null)
            {
                throw new NullReferenceException($"User with id={user.Id} not found");
            }
            UserRoleDTO userRoleDTO = _mapper.Map<UserRoleDTO>(user);
            return userRoleDTO;
        }

        public async Task<Role> GetUserRole(Guid userId)
        {
            if (!await _appContext.Users.AnyAsync(u => Guid.Equals(u.Id, userId)))
            {
                throw new NullReferenceException($"Event with id={userId} not found");
            }
            return await _appContext.Users.Include(u => u.Role).Where(u => u.Id == userId).Select(u => u.Role).FirstOrDefaultAsync();
        }

        public async Task ChangePassword(ChangePasswordDTO changePasswordDTO)
        {
            User user = await _appContext.Users.FirstOrDefaultAsync(u => Guid.Equals(u.Id, Guid.Parse(changePasswordDTO.Id)));
            if (user == null)
            {
                throw new NullReferenceException($"User with id={user.Id} not found");
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
