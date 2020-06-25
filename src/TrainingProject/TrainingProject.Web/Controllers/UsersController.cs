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
        private IUserManager _userManager;
        private IHostServices _hostServices;
        private ILogHelper _logger;
        public UsersController(IUserManager userManager, IHostServices hostServices, ILogHelper logger)
        {
            _userManager = userManager;
            _hostServices = hostServices;
            _logger = logger;
        }
        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin,Account manager")]
        public async Task<ActionResult<Page<UserLiteDTO>>> IndexAsync([FromQuery] int index = 0, int pageSize = 20, string search = null)
        {
            _logger.LogMethodCallingWithObject(new { index, pageSize, search});
            var role = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultRoleClaimType))?.Value;
            if (role != "Admin" && role != "Account manager")
            {
                return Forbid("Access denied");
            }
            return Ok(await _userManager.GetUsersAsync(index, pageSize, search));
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserFullDTO>> DetailsAsync(string userId)
        {
            _logger.LogMethodCallingWithObject(new { userId });
            var hostRoot = _hostServices.GetHostPath();
            return Ok(await _userManager.GetUserAsync(Guid.Parse(userId)));
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] RegisterDTO registerDTO)
        {
            _logger.LogMethodCallingWithObject(registerDTO, "Password, PasswordConfirm");
            if (ModelState.IsValid)
            {
                await _userManager.RegisterUserAsync(registerDTO);
                return Ok();
            }
            return BadRequest("Model state is not valid");
        }

        [HttpGet("{userId}/update")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<UserToUpdateDTO>> UpdateAsync(string userId)
        {
            _logger.LogMethodCallingWithObject(new { userId });
            var role = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultRoleClaimType))?.Value;
            var currentUserId = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultNameClaimType))?.Value;
            if (role != "Admin" && currentUserId != userId)
            {
                return Forbid("Access denied");
            }
            var hostRoot = _hostServices.GetHostPath();
            return Ok(await _userManager.GetUserToUpdateAsync(Guid.Parse(userId)));
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> UpdateAsync([FromForm] UserUpdateDTO userUpdateDTO)
        {
            _logger.LogMethodCallingWithObject(userUpdateDTO);
            var role = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultRoleClaimType))?.Value;
            var userId = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultNameClaimType))?.Value;
            if (role != "Admin" && userId != userUpdateDTO.Id)
            {
                return Forbid("Access denied");
            }
            if (ModelState.IsValid)
            {
                var hostRoot = _hostServices.GetHostPath();
                await _userManager.UpdateUserAsync(userUpdateDTO, hostRoot);
                return Ok();
            }
            return BadRequest("Model state is not valid");
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
        public async Task<ActionResult<UserToBanDTO>> BanAsync(string userId)
        {
            _logger.LogMethodCallingWithObject(new { userId });
            var currentRole = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultRoleClaimType))?.Value;
            var userRole = (await _userManager.GetUserRoleAsync(Guid.Parse(userId))).ToString();
            if (userRole == "Account manager" || userRole == currentRole)
            {
                return Forbid("Lack of rights");
            }
            return Ok(await _userManager.GetUserToBanAsync(Guid.Parse(userId)));
        }

        [HttpPut]
        [Route("ban")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin,Account manager")]
        public async Task<ActionResult> BanAsync([FromBody]BanDTO banDTO)
        {
            _logger.LogMethodCallingWithObject(banDTO);
            var currentRole = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultRoleClaimType))?.Value;
            var userRole = (await _userManager.GetUserRoleAsync(Guid.Parse(banDTO.Id))).ToString();
            if (userRole == "Account manager" || userRole == currentRole)
            {
                return Forbid("Lack of rights");
            }
            if (ModelState.IsValid)
            {
                await _userManager.BanUserAsync(banDTO);
                return Ok();
            }
            return BadRequest("Model state is not valid");
        }

        [HttpPut("{userId}/unban")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin,Account manager")]
        public async Task<ActionResult> UnbanAsync(string userId)
        {
            _logger.LogMethodCallingWithObject(new { userId });
            if (!Guid.TryParse(userId, out Guid userGuid))
            {
                return BadRequest("Invalid user id");
            }
            var currentRole = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultRoleClaimType))?.Value;
            var userRole = (await _userManager.GetUserRoleAsync(userGuid)).ToString();
            if (userRole == "Account manager" || userRole == currentRole)
            {
                return Forbid("Lack of rights");
            }
            await _userManager.UnbanUserAsync(userGuid);
            return Ok();
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
        public async Task<ActionResult<UserRoleDTO>> ChangeRoleAsync(string userId)
        {
            _logger.LogMethodCallingWithObject(new { userId });
            return Ok(await _userManager.GetUserWithRoleAsync(Guid.Parse(userId)));
        }

        [HttpPut]
        [Route("role")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Account manager")]
        public async Task<ActionResult> ChangeRoleAsync([FromBody] ChangeRoleDTO changeRoleDTO)
        {
            _logger.LogMethodCallingWithObject(changeRoleDTO);
            if (ModelState.IsValid)
            {
                await _userManager.ChangeRoleAsync(changeRoleDTO);
                return Ok();
            }
            return BadRequest("Model state is not valid");
        }

        [HttpPut]
        [Route("changePassword")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> ChangePasswordAsync([FromBody]ChangePasswordDTO changePasswordDTO)
        {
            _logger.LogMethodCallingWithObject(changePasswordDTO, "OldPassword, NewPassword, NewPasswordConfirm");
            if (ModelState.IsValid)
            {
                await _userManager.ChangePasswordAsync(changePasswordDTO);
                return Ok();
            }
            return BadRequest("Model state is not valid");
        }

        [HttpPost]
        [Route("signIn")]
        public async Task<ActionResult> SignInAsync([FromBody] LoginDTO loginDTO)
        {
            _logger.LogMethodCallingWithObject(loginDTO, "Password");
            if (ModelState.IsValid)
            {
                return Ok(await _userManager.LoginAsync(loginDTO));
            }
            return BadRequest("Model state is not valid");
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<IActionResult> RefreshAsync([FromBody] RefreshDto refreshDto)
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
        public async Task<IActionResult> LogOut(string userId)
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
    }
}
