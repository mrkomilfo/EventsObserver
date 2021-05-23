using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TrainingProject.Common;
using TrainingProject.Domain;
using TrainingProject.DomainLogic.Interfaces;
using TrainingProject.DomainLogic.Models.Common;
using TrainingProject.DomainLogic.Models.Users;
using TrainingProject.Web.Filters;
using TrainingProject.Web.Interfaces;

namespace TrainingProject.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(ExceptionHandlingFilter))]
    public class UsersController : ControllerBase
    {
        private readonly IUserManager _userManager;
        private readonly IHostServices _hostServices;
        private readonly ILogHelper _logger;

        public UsersController(IUserManager userManager, IHostServices hostServices, ILogHelper logger)
        {
            _userManager = userManager;
            _hostServices = hostServices;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin,Account manager")]
        public async Task<ActionResult<Page<UserLiteDto>>> IndexAsync([FromQuery] int index = 0, int pageSize = 20, string search = null)
        {
            _logger.LogMethodCallingWithObject(new { index, pageSize, search });
            var role = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultRoleClaimType))?.Value;
            if (role != "Admin" && role != "Account manager")
            {
                return Forbid("Access denied");
            }
            return Ok(await _userManager.GetUsersAsync(index, pageSize, search));
        }

        [HttpGet]
        [Route("resetPassword")]
        public async Task<IActionResult> ResetPassword([FromQuery] string email)
        {
            _logger.LogMethodCallingWithObject(new { email });
            await _userManager.RequestPasswordResetAsync(email);
            return Ok();
        }

        [HttpPut]
        [Route("resetPassword")]
        public async Task<IActionResult> ResetPassword([FromQuery] string email, string confirmCode)
        {
            _logger.LogMethodCallingWithObject(new { email });
            await _userManager.ResetPasswordAsync(email, confirmCode);
            return Ok();
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserFullDto>> DetailsAsync(string userId)
        {
            _logger.LogMethodCallingWithObject(new { userId });
            return Ok(await _userManager.GetUserAsync(Guid.Parse(userId)));
        }

        [HttpPost]
        [ModelStateValidation]
        public async Task<ActionResult> CreateAsync([FromBody] RegisterDto registerDto)
        {
            _logger.LogMethodCallingWithObject(registerDto, "Password, PasswordConfirm");
            await _userManager.RegisterUserAsync(registerDto);
            return Ok();
        }

        [HttpGet("{userId}/update")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<UserToUpdateDto>> UpdateAsync(string userId)
        {
            _logger.LogMethodCallingWithObject(new { userId });
            var role = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultRoleClaimType))?.Value;
            var currentUserId = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultNameClaimType))?.Value;
            if (Equals(role, "Admin") && !Equals(currentUserId, userId))
            {
                return Forbid("Access denied");
            }
            var hostRoot = _hostServices.GetHostPath();
            return Ok(await _userManager.GetUserToUpdateAsync(Guid.Parse(userId)));
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ModelStateValidation]
        public async Task<ActionResult> UpdateAsync([FromForm] UserUpdateDto userUpdateDto)
        {
            _logger.LogMethodCallingWithObject(userUpdateDto);
            var role = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultRoleClaimType))?.Value;
            var userId = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultNameClaimType))?.Value;
            if (!Equals(role, "Admin") && !Equals(userId, userUpdateDto.Id))
            {
                return Forbid("Access denied");
            }
            var hostRoot = _hostServices.GetHostPath();
            await _userManager.UpdateUserAsync(userUpdateDto, hostRoot);
            return Ok();
        }

        [HttpDelete("{userId}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Account manager")]
        public async Task<ActionResult> DeleteAsync(string userId)
        {
            _logger.LogMethodCallingWithObject(new { userId });
            var hostRoot = _hostServices.GetHostPath();
            await _userManager.DeleteUserAsync(Guid.Parse(userId), false, hostRoot);
            return Ok();
        }

        [HttpGet("{userId}/ban")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin,Account manager")]
        public async Task<ActionResult<UserToBanDto>> BanAsync(string userId)
        {
            _logger.LogMethodCallingWithObject(new { userId });
            var currentRole = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultRoleClaimType))?.Value;
            var userRole = (await _userManager.GetUserRoleAsync(Guid.Parse(userId))).ToString();
            if (Equals(userRole, "Account manager") || Equals(userRole, currentRole))
            {
                return Forbid("Lack of rights");
            }
            return Ok(await _userManager.GetUserToBanAsync(Guid.Parse(userId)));
        }

        [HttpPut]
        [Route("ban")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin,Account manager")]
        [ModelStateValidation]
        public async Task<ActionResult> BanAsync([FromBody] BanDto banDto)
        {
            _logger.LogMethodCallingWithObject(banDto);
            var currentRole = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultRoleClaimType))?.Value;
            var userRole = (await _userManager.GetUserRoleAsync(Guid.Parse(banDto.Id))).ToString();
            if (Equals(userRole, "Account manager") || Equals(userRole, currentRole))
            {
                return Forbid("Lack of rights");
            }
            await _userManager.BanUserAsync(banDto);
            return Ok();
        }

        [HttpPut("{userId}/unban")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin,Account manager")]
        public async Task<ActionResult> UnbanAsync(string userId)
        {
            _logger.LogMethodCallingWithObject(new { userId });
            var currentRole = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultRoleClaimType))?.Value;
            var userRole = (await _userManager.GetUserRoleAsync(Guid.Parse(userId))).ToString();
            if (Equals(userRole, "Account manager") || Equals(userRole, currentRole))
            {
                return Forbid("Lack of rights");
            }
            await _userManager.UnbanUserAsync(Guid.Parse(userId));
            return Ok();
        }

        [HttpGet]
        [Route("blockingExpiration")]
        public async Task<ActionResult<string>> BlockingExpirationAsync([FromQuery] string email)
        {
            _logger.LogMethodCallingWithObject(new { email });
            return Ok(await _userManager.GetBlockingExpirationAsync(email));
        }

        [HttpGet]
        [Route("roles")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Account manager")]
        public async Task<ActionResult<IEnumerable<Role>>> RolesAsync()
        {
            _logger.LogMethodCalling();
            return Ok(await _userManager.GetRolesAsync());
        }

        [HttpGet("{userId}/role")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Account manager")]
        public async Task<ActionResult<UserRoleDto>> ChangeRoleAsync(string userId)
        {
            _logger.LogMethodCallingWithObject(new { userId });
            return Ok(await _userManager.GetUserWithRoleAsync(Guid.Parse(userId)));
        }

        [HttpPut]
        [Route("role")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Account manager")]
        [ModelStateValidation]
        public async Task<ActionResult> ChangeRoleAsync([FromBody] ChangeRoleDto changeRoleDto)
        {
            _logger.LogMethodCallingWithObject(changeRoleDto);
            await _userManager.ChangeRoleAsync(changeRoleDto);
            return Ok();
        }

        [HttpPut]
        [Route("changePassword")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ModelStateValidation]
        public async Task<ActionResult> ChangePasswordAsync([FromBody] ChangePasswordDto changePasswordDto)
        {
            _logger.LogMethodCallingWithObject(changePasswordDto, "OldPassword, NewPassword, NewPasswordConfirm");
            await _userManager.ChangePasswordAsync(changePasswordDto);
            return Ok();
        }

        [HttpPost]
        [Route("signIn")]
        [ModelStateValidation]
        public async Task<ActionResult> SignInAsync([FromBody] LoginDto emailDto)
        {
            _logger.LogMethodCallingWithObject(emailDto, "Password");
            return Ok(await _userManager.LoginAsync(emailDto));
        }

        [HttpPost]
        [Route("refresh")]
        [ModelStateValidation]
        public async Task<ActionResult> RefreshAsync([FromBody] RefreshDto refreshDto)
        {
            _logger.LogMethodCallingWithObject(refreshDto);
            var principal = _userManager.GetPrincipalFromExpiredToken(refreshDto.Token);
            var userId = principal.Identity.Name;
            var savedRefreshToken = await _userManager.GetRefreshTokenAsync(userId);
            if (savedRefreshToken != refreshDto.RefreshToken || string.IsNullOrEmpty(savedRefreshToken))
                throw new SecurityTokenException("Invalid refresh token");

            var newJwtToken = _userManager.GenerateToken(principal.Claims);
            var newRefreshToken = _userManager.GenerateRefreshToken();
            await _userManager.SaveRefreshTokenAsync(userId, newRefreshToken);

            return new ObjectResult(new
            {
                accessToken = newJwtToken,
                refreshToken = newRefreshToken
            });
        }

        [HttpPut]
        [Route("{userId}/logout")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> LogOut(string userId)
        {
            _logger.LogMethodCallingWithObject(new { userId });
            var role = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultRoleClaimType))?.Value;
            var currentUserId = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultNameClaimType))?.Value;
            if (role != "Admin" && role != "Account manager" && !Equals(Guid.Parse(userId), currentUserId))
            {
                return Forbid("Access denied");
            }
            await _userManager.DeleteRefreshTokenAsync(userId);
            return Ok();
        }

        [HttpGet]
        [Route("{email}/confirmEmail")]
        public async Task<ActionResult> ConfirmEmail(string email)
        {
            _logger.LogMethodCallingWithObject(new { email });

            await _userManager.RequestEmailConfirmAsync(email);

            return Ok();
        }

        [HttpPut]
        [Route("{email}/confirmEmail")]
        public async Task<ActionResult> ConfirmEmail(string email, [FromQuery] string confirmCode)
        {
            _logger.LogMethodCallingWithObject(new {userId = email });

            await _userManager.ConfirmEmailAsync(email, confirmCode);

            return Ok();
        }
    }
}
