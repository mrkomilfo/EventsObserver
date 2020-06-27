using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TrainingProject.Domain;
using TrainingProject.DomainLogic.Models.Common;
using TrainingProject.DomainLogic.Models.Users;

namespace TrainingProject.DomainLogic.Interfaces
{
    public interface IUserManager
    {
        Task<UserFullDto> GetUserAsync(Guid userId);
        Task<Page<UserLiteDto>> GetUsersAsync(int index, int pageSize, string search);
        Task RegisterUserAsync(RegisterDto user);
        Task UpdateUserAsync(UserUpdateDto user, string hostRoot);
        Task<UserToUpdateDto> GetUserToUpdateAsync(Guid userId);
        Task DeleteUserAsync(Guid userId, bool force, string hostRoot);
        Task<UserToBanDto> GetUserToBanAsync(Guid userId);
        Task BanUserAsync(BanDto banDto);
        Task UnbanUserAsync(Guid userId);
        Task<DateTime?> GetUnlockTimeAsync(Guid userId);
        Task<LoginResponseDto> LoginAsync(LoginDto loginDto);
        Task ChangeRoleAsync(ChangeRoleDto changeRoleDto);
        Task<IEnumerable<Role>> GetRolesAsync();
        Task<UserRoleDto> GetUserWithRoleAsync(Guid userId);
        Task<Role> GetUserRoleAsync(Guid userId);
        Task ChangePasswordAsync(ChangePasswordDto changePasswordDto);
        Task<string> GetUserNameAsync(Guid guid);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        Task<string> GetRefreshTokenAsync(string userId);
        string GenerateToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
        Task DeleteRefreshTokenAsync(string userId);
        Task SaveRefreshTokenAsync(string userId, string newRefreshToken);
    }
}
