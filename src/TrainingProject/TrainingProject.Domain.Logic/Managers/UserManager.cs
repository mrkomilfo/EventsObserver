using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
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

        public async Task<UserFullDto> GetUserAsync(Guid userId)
        {
            _logger.LogMethodCallingWithObject(new { userId });
            var DBUser = await _appContext.Users.Include(u => u.Role).Include(u => u.OrganizedEvents).FirstOrDefaultAsync(u => u.Id == userId);
            if (DBUser == null)
            {
                throw new KeyNotFoundException($"User with id={userId} not found");
            }
            var user = _mapper.Map<UserFullDto>(DBUser);
            user.VisitedEvents = await _appContext.EventsUsers.Where(eu => eu.ParticipantId == userId).CountAsync();

            string imageName = DBUser.HasPhoto ? userId.ToString() : "default";
            string path = $"img\\users\\{imageName}.jpg";
            user.Photo = path;

            return user;
        }

        public async Task<Page<UserLiteDto>> GetUsersAsync(int index, int pageSize, string search)
        {
            _logger.LogMethodCallingWithObject(new { index, pageSize, search });
            var result = new Page<UserLiteDto>() { CurrentPage = index, PageSize = pageSize };
            var query = _appContext.Users.Include(e => e.Role).AsQueryable();
            if (search != null)
            {
                query = query.Where(u => u.UserName.ToLower().Contains(search.ToLower()));
            }
            result.TotalRecords = await query.CountAsync();
            result.Records = await _mapper.ProjectTo<UserLiteDto>(query).ToListAsync(default);
            return result;
        }

        public async Task RegisterUserAsync(RegisterDto user)
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

        public async Task UpdateUserAsync(UserUpdateDto user, string hostRoot)
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

        public async Task<UserToUpdateDto> GetUserToUpdateAsync(Guid userId)
        {
            _logger.LogMethodCallingWithObject(new { userId });
            User user = await _appContext.Users.FindAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with id={user.Id} not found");
            }
            UserToUpdateDto userToUpdate = _mapper.Map<UserToUpdateDto>(user);
            if (userToUpdate.HasPhoto)
            {
                userToUpdate.Photo = $"img\\users\\{userId}.jpg";
            }
            return userToUpdate;
        }

        public async Task DeleteUserAsync(Guid userId, bool force, string hostRoot)
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

        public async Task<UserToBanDto> GetUserToBanAsync(Guid userId)
        {
            _logger.LogMethodCallingWithObject(new { userId });
            var user = await _appContext.Users.FirstOrDefaultAsync(u => Equals(u.Id, userId));
            if (user == null)
            {
                throw new KeyNotFoundException($"User with id={user.Id} not found");
            }
            UserToBanDto userToBan = _mapper.Map<UserToBanDto>(user);
            return userToBan;
        }

        public async Task BanUserAsync(BanDto banDto)
        {
            _logger.LogMethodCallingWithObject(banDto);
            var user = await _appContext.Users.FirstOrDefaultAsync(u => Equals(u.Id, Guid.Parse(banDto.Id)));
            if (user == null)
            {
                throw new KeyNotFoundException($"User with id={user.Id} not found");
            }
            user.UnlockTime = DateTime.Now.AddDays(banDto?.Days ?? 0);
            user.UnlockTime = user.UnlockTime?.AddHours(banDto?.Hours ?? 0);
            await _appContext.SaveChangesAsync(default);
            await DeleteRefreshTokenAsync(banDto.Id);
        }

        public async Task UnbanUserAsync(Guid userId)
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

        public async Task ChangeRoleAsync(ChangeRoleDto changeRoleDto)
        {
            _logger.LogMethodCallingWithObject(changeRoleDto);
            var user = await _appContext.Users.FindAsync(Guid.Parse(changeRoleDto.UserId));
            if (user == null)
            {
                throw new KeyNotFoundException($"User with id={user.Id} not found");
            }
            if (!await _appContext.Roles.AnyAsync(r => r.Id == changeRoleDto.RoleId))
            {
                throw new KeyNotFoundException($"Role with id={changeRoleDto.RoleId} not found");
            }
            user.RoleId = changeRoleDto.RoleId;
            await _appContext.SaveChangesAsync(default);
            await DeleteRefreshTokenAsync(changeRoleDto.UserId);
        }
        public async Task<DateTime?> GetUnlockTimeAsync(Guid userId)
        {
            _logger.LogMethodCallingWithObject(new { userId });
            if (!await _appContext.Users.AnyAsync(u => Equals(u.Id, userId)))
            {
                throw new KeyNotFoundException($"User with id={userId} not found");
            }
            return (await _appContext.Users.FirstAsync(u => Equals(u.Id, userId))).UnlockTime;
        }

        public async Task<IEnumerable<Role>> GetRolesAsync()
        {
            _logger.LogMethodCalling();
            return await _appContext.Roles.ToListAsync();
        }

        public async Task<UserRoleDto> GetUserWithRoleAsync(Guid userId)
        {
            _logger.LogMethodCallingWithObject(new { userId });
            User user = await _appContext.Users.FirstOrDefaultAsync(u => Equals(u.Id, userId));
            if (user == null)
            {
                throw new KeyNotFoundException($"User with id={user.Id} not found");
            }
            UserRoleDto userRoleDto = _mapper.Map<UserRoleDto>(user);
            return userRoleDto;
        }

        public async Task<string> GetUserNameAsync(Guid userId)
        {
            _logger.LogMethodCallingWithObject(new { userId });
            if (!await _appContext.Users.AnyAsync(u => Equals(u.Id, userId)))
            {
                throw new KeyNotFoundException($"Event with id={userId} not found");
            }
            return await _appContext.Users.Where(u => Equals(u.Id, userId)).Select(u => u.UserName).FirstOrDefaultAsync();
        }

        public async Task<Role> GetUserRoleAsync(Guid userId)
        {
            _logger.LogMethodCallingWithObject(new { userId });
            if (!await _appContext.Users.AnyAsync(u => Equals(u.Id, userId)))
            {
                throw new KeyNotFoundException($"Event with id={userId} not found");
            }
            return await _appContext.Users.Include(u => u.Role).Where(u => Equals(u.Id, userId)).Select(u => u.Role).FirstOrDefaultAsync();
        }

        public async Task ChangePasswordAsync(ChangePasswordDto changePasswordDto)
        {
            _logger.LogMethodCallingWithObject(changePasswordDto, "OldPassword, NewPassword, NewPasswordConfirm");
            User user = await _appContext.Users.FirstOrDefaultAsync(u => Equals(u.Id, Guid.Parse(changePasswordDto.Id)));
            if (user == null)
            {
                throw new KeyNotFoundException($"User with id={user.Id} not found");
            }
            if (!String.Equals(HashGenerator.Encrypt(changePasswordDto.OldPassword), user.Password))
            {
                throw new ArgumentException("Wrong old password");
            }
            user.Password = HashGenerator.Encrypt(changePasswordDto.NewPassword);
            await _appContext.SaveChangesAsync(default);
            await DeleteRefreshTokenAsync(changePasswordDto.Id);
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

        public async Task<string> GetRefreshTokenAsync(string userId)
        {
            _logger.LogMethodCallingWithObject(new { userId });
            User user = await _appContext.Users.FirstOrDefaultAsync(u => Equals(u.Id, Guid.Parse(userId)));
            if (user == null)
            {
                throw new KeyNotFoundException($"User with id={userId} not found");
            }
            return user.RefreshToken;
        }

        public async Task SaveRefreshTokenAsync(string userId, string refreshToken)
        {
            _logger.LogMethodCallingWithObject(new { userId, refreshToken });
            User user = await _appContext.Users.FirstOrDefaultAsync(u => Equals(u.Id, Guid.Parse(userId)));
            if (user == null)
            {
                throw new KeyNotFoundException($"User with id={userId} not found");
            }
            user.RefreshToken = refreshToken;
            await _appContext.SaveChangesAsync(default);
        }

        public async Task DeleteRefreshTokenAsync(string userId)
        {
            _logger.LogMethodCallingWithObject(new { userId });
            User user = await _appContext.Users.FirstOrDefaultAsync(u => Equals(u.Id, Guid.Parse(userId)));
            if (user == null)
            {
                throw new KeyNotFoundException($"User with id={userId} not found");
            }
            user.RefreshToken = null;
            await _appContext.SaveChangesAsync(default);
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
        {
            _logger.LogMethodCallingWithObject(loginDto, "Password");
            var identity = await GetIdentity(loginDto.Login, loginDto.Password);
            if (identity == null)
            {
                throw new UnauthorizedAccessException($"Wrong login or password");
            }
            var unlockTime = await GetUnlockTimeAsync(Guid.Parse(identity.Name));
            if (unlockTime != null && ((unlockTime ?? DateTime.Now) > DateTime.Now))
            {
                throw new UnauthorizedAccessException($"Banned until {unlockTime?.ToString("f")}");
            }

            var accessToken = GenerateToken(identity.Claims);
            var refreshToken = GenerateRefreshToken();
            await SaveRefreshTokenAsync(identity.Name, refreshToken);
            var response = new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Name = identity.Name,
                Role = identity.Claims.Where(c => c.Type == ClaimTypes.Role).FirstOrDefault().Value,
            };
            return response;
        }

        public string GenerateToken(IEnumerable<Claim> claims)
        {
            _logger.LogMethodCallingWithObject(new 
            { 
                claims = String.Join(", ", claims.ToList().ConvertAll(delegate (Claim c) { return c.ToString(); }).ToArray())
            });
            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt); //the method is called WriteToken but returns a string
        }

        public string GenerateRefreshToken()
        {
            _logger.LogMethodCalling();
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            _logger.LogMethodCallingWithObject(new { token });
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = AuthOptions.ISSUER,
                ValidAudience = AuthOptions.AUDIENCE,
                IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                ValidateIssuerSigningKey = true,
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (!(securityToken is JwtSecurityToken jwtSecurityToken)
                || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }
    }
}
