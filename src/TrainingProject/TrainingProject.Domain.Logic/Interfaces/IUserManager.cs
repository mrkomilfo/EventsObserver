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
        Task<UserFullDTO> GetUserAsync(Guid userId);
        Task<Page<UserLiteDTO>> GetUsersAsync(int index, int pageSize, string search);
        Task RegisterUserAsync(RegisterDTO user);
        Task UpdateUserAsync(UserUpdateDTO user, string hostRoot);
        Task<UserToUpdateDTO> GetUserToUpdateAsync(Guid userId);
        Task DeleteUserAsync(Guid userId, bool force, string hostRoot);
        Task<UserToBanDTO> GetUserToBanAsync(Guid userId);
        Task BanUserAsync(BanDTO banDTO);
        Task UnbanUserAsync(Guid userId);
        Task<DateTime?> GetUnlockTimeAsync(Guid userId);
        Task<LoginResponseDTO> LoginAsync(LoginDTO loginDto);
        Task ChangeRoleAsync(ChangeRoleDTO changeRoleDTO);
        Task<IEnumerable<Role>> GetRolesAsync();
        Task<UserRoleDTO> GetUserWithRoleAsync(Guid userId);
        Task<Role> GetUserRoleAsync(Guid userId);
        Task ChangePasswordAsync(ChangePasswordDTO changePasswordDTO);
        Task<string> GetUserNameAsync(Guid guid);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        Task<string> GetRefreshTokenAsync(string userId);
        string GenerateToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
        Task DeleteRefreshTokenAsync(string userId);
        Task SaveRefreshTokenAsync(string userId, string newRefreshToken);
    }
}
