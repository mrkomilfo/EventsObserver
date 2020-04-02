using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TrainingProject.Data;
using TrainingProject.Domain;
using TrainingProject.DomainLogic.Helpers;
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

        public async Task<Maybe<UserFullDTO>> GetUser(Guid userId, string hostRoot)
        {
            var DBUser = await _appContext.Users.Include(u => u.Role).Include(u=>u.OrganizedEvents).FirstOrDefaultAsync(u => u.Id == userId);
            var user = _mapper.Map<UserFullDTO>(DBUser);
            user.VisitedEvents = await _appContext.EventsUsers.Where(eu => eu.ParticipantId == userId).CountAsync();

            string imageName = DBUser.HasPhoto ? userId.ToString() : "default";
            string path = $"{hostRoot}/img/users/{imageName}.jpg";
            user.Photo = path;

            return user;
        }

        public async Task<Page<UserLiteDTO>> GetUsers(int index, int pageSize)
        {
            var result = new Page<UserLiteDTO>() { CurrentPage = index, PageSize = pageSize };
            var query = _appContext.Users.Include(e => e.Role).AsQueryable();
            result.TotalRecords = await query.CountAsync();
            result.Records = await _mapper.ProjectTo<UserLiteDTO>(query).ToListAsync(default);
            return result;
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

        public async Task UpdateUser(UserUpdateDTO user, string hostRoot)
        {
            User updatedUser = await _appContext.Users.FindAsync(user.Id);
            if (updatedUser != null)
            {
                _mapper.Map(user, updatedUser);
                if (user.Photo != null)
                {
                    string path = $"{hostRoot}/img/users/{updatedUser.Id}.jpg";
                    await using var fileStream = new FileStream(path, FileMode.Create);
                    await user.Photo.CopyToAsync(fileStream);
                }
            }

            await _appContext.SaveChangesAsync(default);
        }

        public async Task DeleteUser(Guid userId, bool force, string hostRoot)
        {
            var user = await _appContext.Users.IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null)
            {
                if (force)
                {
                    _appContext.Users.Remove(user);
                    string path = $"{hostRoot}/img/users/{userId}.jpg";
                    File.Delete(path);
                }
                else
                {
                    user.IsDeleted = true;
                }
                await _appContext.SaveChangesAsync(default);
            }
        }

        public async Task BanUser(BanDTO banDTO)
        {
            var user = await _appContext.Users.FirstOrDefaultAsync(u => u.Id == Guid.Parse(banDTO.UserId));
            if (user != null)
            {
                user.UnlockTime = DateTime.Now.AddDays(banDTO?.Days ?? 0);
                user.UnlockTime = DateTime.Now.AddHours(banDTO?.Hours ?? 0);
            }
            await _appContext.SaveChangesAsync(default);
        }

        public async Task UnbanUser(Guid userId)
        {
            var user = await _appContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null)
            {
                user.UnlockTime = DateTime.Now;
            }
            await _appContext.SaveChangesAsync(default);
        }

        public async Task ChangeRole(Guid userId, int roleId)
        {
            var user = await _appContext.Users.FindAsync(userId);
            if (user != null)
            {
                user.RoleId = roleId;
            }
            await _appContext.SaveChangesAsync(default);
        }
        public DateTime? GetUnlockTime(Guid userId)
        {
            return _appContext.Users.FirstOrDefault(u => u.Id == userId)?.UnlockTime;
        }
        public async Task<Maybe<LoginResponseDTO>> Login(LoginDTO model)
        {
            var identity = await GetIdentity(model.Login, model.Password);
            if (identity == null)
            {
                return new LoginResponseDTO
                {
                    Status = Status.WrongLoginOrPassword
                };
            }
            var unlockTime = GetUnlockTime(Guid.Parse(identity.Name));
            if (unlockTime != null && ((unlockTime ?? DateTime.Now) > DateTime.Now))
            {
                return new LoginResponseDTO
                {
                    Status = Status.Blocked,
                    Blocking = unlockTime ?? DateTime.Now,
                };
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
                //Name = identity.Name,
                //Role = identity.Claims.Where(c => c.Type == ClaimTypes.Role).FirstOrDefault(),
                Status = Status.Ok
            };
            return response;
        }
        private async Task<ClaimsIdentity> GetIdentity(string login, string password)
        {
            ClaimsIdentity identity = null;
            var user = await _appContext.Users.Include(u=>u.Role).FirstOrDefaultAsync(u => u.Login == login);
            if (user != null)
            {
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
            }
            return identity;
        }
    }
}
