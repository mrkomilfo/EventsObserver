using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using TrainingProject.Common;
using TrainingProject.Data;
using TrainingProject.Domain;
using TrainingProject.DomainLogic.Helpers;
using TrainingProject.DomainLogic.Interfaces;
using TrainingProject.DomainLogic.Models.Common;
using TrainingProject.DomainLogic.Models.Users;
using TrainingProject.DomainLogic.Services;
using TrainingProject.Web.Helpers;

namespace TrainingProject.DomainLogic.Managers
{
    public class UserManager : IUserManager
    {
        private readonly IAppContext _appContext;
        private readonly IMapper _mapper;
        private readonly ILogHelper _logger;
        private readonly INotificator _notificator;

        private readonly Random _random = new Random();

        private const int KeyLength = 8;
        private const string Fmt = "ddMMyyyyHHmmss";

        public UserManager(IAppContext appContext, IMapper mapper, ILogHelper logger, INotificator notificator)
        {
            _appContext = appContext;
            _mapper = mapper;
            _logger = logger;
            _notificator = notificator;
        }

        public async Task<UserFullDto> GetUserAsync(Guid userId)
        {
            _logger.LogMethodCallingWithObject(new { userId });
            var dbUser = await _appContext.Users.Include(u => u.Role).Include(u => u.OrganizedEvents).FirstOrDefaultAsync(u => Equals(u.Id, userId));
            if (dbUser == null)
            {
                throw new KeyNotFoundException($"User with id={userId} not found");
            }
            var user = _mapper.Map<UserFullDto>(dbUser);
            user.VisitedEvents = await _appContext.EventsParticipants.Where(eu => Equals(eu.ParticipantId, userId)).CountAsync();

            var imageName = dbUser.HasPhoto ? userId.ToString() : "default";
            var path = $"img\\users\\{imageName}.jpg";
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
                throw new ArgumentOutOfRangeException($"User with login \"{user.Login}\" already exist");
            }
            User newUser = _mapper.Map<User>(user);
            newUser.RoleId = (await _appContext.Roles.FirstOrDefaultAsync(r => r.Name == "User"))?.Id;
            await _appContext.Users.AddAsync(newUser);
            await _appContext.SaveChangesAsync(default);
        }

        public async Task UpdateUserAsync(UserUpdateDto user, string hostRoot)
        {
            _logger.LogMethodCallingWithObject(user);
            var updatedUser = await _appContext.Users.FirstOrDefaultAsync(u => Equals(u.Id, Guid.Parse(user.Id)));
            if (updatedUser == null)
            {
                throw new KeyNotFoundException($"User with id={user.Id} not found");
            }
            if (!string.Equals(user.ContactEmail, updatedUser.ContactEmail))
            {
                updatedUser.EmailConfirmed = false;
            }
            _mapper.Map(user, updatedUser);
            if (user.Photo != null)
            {
                var path = $"{hostRoot}\\wwwroot\\img\\users\\{updatedUser.Id}.jpg";
                _logger.LogInfo($"Saving image to {path}");
                await using var fileStream = new FileStream(path, FileMode.Create);
                await user.Photo.CopyToAsync(fileStream);
            }
            await _appContext.SaveChangesAsync(default);
        }

        public async Task<UserToUpdateDto> GetUserToUpdateAsync(Guid userId)
        {
            _logger.LogMethodCallingWithObject(new { userId });
            var user = await _appContext.Users.FirstOrDefaultAsync(u => Equals(u.Id, userId));
            if (user == null)
            {
                throw new KeyNotFoundException($"User with id={userId} not found");
            }
            var userToUpdate = _mapper.Map<UserToUpdateDto>(user);
            if (userToUpdate.HasPhoto)
            {
                userToUpdate.Photo = $"img\\users\\{userId}.jpg";
            }
            return userToUpdate;
        }

        public async Task DeleteUserAsync(Guid userId, bool force, string hostRoot)
        {
            _logger.LogMethodCallingWithObject(new { userId, force, hostRoot });
            var user = await _appContext.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => Equals(u.Id, userId));
            if (user == null)
            {
                throw new KeyNotFoundException($"User with id={userId} not found");
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
                throw new KeyNotFoundException($"User with id={userId} not found");
            }
            var userToBan = _mapper.Map<UserToBanDto>(user);
            return userToBan;
        }

        public async Task BanUserAsync(BanDto banDto)
        {
            _logger.LogMethodCallingWithObject(banDto);
            var user = await _appContext.Users.FirstOrDefaultAsync(u => Equals(u.Id, Guid.Parse(banDto.Id)));
            if (user == null)
            {
                throw new KeyNotFoundException($"User with id={banDto.Id} not found");
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
                throw new KeyNotFoundException($"User with id={userId} not found");
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
                throw new KeyNotFoundException($"User with id={changeRoleDto.UserId} not found");
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
            var user = await _appContext.Users.FirstOrDefaultAsync(u => Equals(u.Id, userId));
            if (user == null)
            {
                throw new KeyNotFoundException($"User with id={userId} not found");
            }
            var userRoleDto = _mapper.Map<UserRoleDto>(user);
            return userRoleDto;
        }

        public async Task<string> GetUserNameAsync(Guid userId)
        {
            _logger.LogMethodCallingWithObject(new { userId });
            if (!await _appContext.Users.AnyAsync(u => Equals(u.Id, userId)))
            {
                throw new KeyNotFoundException($"User with id={userId} not found");
            }
            return await _appContext.Users.Where(u => Equals(u.Id, userId)).Select(u => u.UserName).FirstOrDefaultAsync();
        }

        public async Task<string> GetBlockingExpirationAsync(string login)
        {
            _logger.LogMethodCallingWithObject(new { login });
            if (!await _appContext.Users.AnyAsync(u => Equals(u.Login, login)))
            {
                throw new KeyNotFoundException($"User '{login}' not found");
            }
            return await _appContext.Users.Where(u => Equals(u.Login, login)).Select(u => u.UnlockTime.ToString().Substring(0,19)).FirstOrDefaultAsync();
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
            var user = await _appContext.Users.FirstOrDefaultAsync(u => Equals(u.Id, Guid.Parse(changePasswordDto.Id)));
            if (user == null)
            {
                throw new KeyNotFoundException($"User with id={changePasswordDto.Id} not found");
            }
            if (!Equals(HashGenerator.Encrypt(changePasswordDto.OldPassword), user.Password))
            {
                throw new ArgumentOutOfRangeException("Wrong old password");
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
            var user = await _appContext.Users.FirstOrDefaultAsync(u => Equals(u.Id, Guid.Parse(userId)));
            if (user == null)
            {
                throw new KeyNotFoundException($"User with id={userId} not found");
            }
            return user.RefreshToken;
        }

        public async Task SaveRefreshTokenAsync(string userId, string refreshToken)
        {
            _logger.LogMethodCallingWithObject(new { userId, refreshToken });
            var user = await _appContext.Users.FirstOrDefaultAsync(u => Equals(u.Id, Guid.Parse(userId)));
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
            var user = await _appContext.Users.FirstOrDefaultAsync(u => Equals(u.Id, Guid.Parse(userId)));
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
                throw new AccessViolationException($"Banned until {unlockTime?.ToString("f")}");
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
                claims = string.Join(", ", claims.ToList().ConvertAll(
                    c => c.ToString()).ToArray())
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
            var principal = tokenHandler.
                ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (!(securityToken is JwtSecurityToken jwtSecurityToken)
                || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        private string GenerateRandomString(int length)
        {
            _logger.LogMethodCallingWithObject(new { length });
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        public async Task RequestEmailConfirmAsync(string userId)
        {
            _logger.LogMethodCallingWithObject(new { userId });

            var user = await _appContext.Users.FirstOrDefaultAsync(u => Equals(u.Id, Guid.Parse(userId)));
            var email = user?.ContactEmail;

            if (string.IsNullOrEmpty(email))
            {
                throw new KeyNotFoundException($"User with id={userId} not exist or email not specified");
            }

            var confirmCode = GenerateRandomString(KeyLength);

            user.EmailConfirmCodeHash = DateTime.Now.ToString(Fmt) + HashGenerator.Encrypt(confirmCode);

            await _appContext.SaveChangesAsync(default);

            var title = "[EventObserver] Подтверждение Email";
            var body = $"<p>Привет, {user.UserName}</p>" +
                $"<p>Код подтверждения: {confirmCode}</p>";
            _notificator.SendMessage(title, body, email);
        }

        public async Task ConfirmEmailAsync(string userId, string confirmCode)
        {
            _logger.LogMethodCallingWithObject(new { userId });

            var user = await _appContext.Users.FirstOrDefaultAsync(u => Equals(u.Id, Guid.Parse(userId)));

            if (user == null)
            {
                throw new KeyNotFoundException($"User with id={userId} not found");
            }

            var storedConfirmCode = user.EmailConfirmCodeHash;

            if (string.IsNullOrEmpty(storedConfirmCode)
                || DateTime.ParseExact(storedConfirmCode.Substring(0, Fmt.Length), Fmt, CultureInfo.InvariantCulture).AddMinutes(5) < DateTime.Now
                || !Equals(storedConfirmCode.Substring(Fmt.Length), HashGenerator.Encrypt(confirmCode)))
            {
                throw new ArgumentException($"Wrong email confirm code");
            }

            user.EmailConfirmCodeHash = null;
            user.EmailConfirmed = true;

            await _appContext.SaveChangesAsync(default);
        }

        public async Task RequestPasswordResetAsync(string login)
        {
            _logger.LogMethodCallingWithObject(new { login });

            var user = await _appContext.Users.FirstOrDefaultAsync(u => Equals(u.Login, login));

            if (user == null)
            {
                throw new KeyNotFoundException($"User '{login}' not found");
            }

            var email = user.ContactEmail;

            if (string.IsNullOrEmpty(email) || !user.EmailConfirmed)
            {
                throw new AccessViolationException($"User doesn't have confirmed email");
            }

            var confirmCode = GenerateRandomString(KeyLength);

            user.PasswordResetCodeHash = DateTime.Now.ToString(Fmt) + HashGenerator.Encrypt(confirmCode);

            await _appContext.SaveChangesAsync(default);

            var title = "[EventObserver] Сброс пароля";
            var body = $"<p>Привет, {user.UserName}</p>" +
                $"<p>Код подтверждения для сброса пароля: {confirmCode}</p>";

            _notificator.SendMessage(title, body, email);
        }

        public async Task ResetPasswordAsync(string login, string confirmCode)
        {
            _logger.LogMethodCallingWithObject(new { login });

            var user = await _appContext.Users.FirstOrDefaultAsync(u => Equals(u.Login, login));

            if (user == null)
            {
                throw new KeyNotFoundException($"User '{login}' not found");
            }

            var storedConfirmCode = user.PasswordResetCodeHash;

            if (string.IsNullOrEmpty(storedConfirmCode)
                || DateTime.ParseExact(storedConfirmCode[..Fmt.Length], Fmt, CultureInfo.InvariantCulture).AddMinutes(5) < DateTime.Now
                || !Equals(storedConfirmCode[Fmt.Length..], HashGenerator.Encrypt(confirmCode)))
            {
                throw new UnauthorizedAccessException($"Wrong password reset code");
            }

            var newPassword = GenerateRandomString(KeyLength);

            user.PasswordResetCodeHash = null;
            user.Password = HashGenerator.Encrypt(newPassword);

            await _appContext.SaveChangesAsync(default);

            var title = "[EventObserver] Восстановление пароля";
            var body = $"<p>Привет, {user.UserName}</p>" +
                $"<p>Ваш новый пароль: {newPassword}</p>" +
                $"<p>Пожалуйста, измените его при первом входе</p>";

            _notificator.SendMessage(title, body, user.ContactEmail);
        }
    }
}
