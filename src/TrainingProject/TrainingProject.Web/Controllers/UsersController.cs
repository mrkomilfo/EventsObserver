using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TrainingProject.Domain;
using TrainingProject.DomainLogic.Interfaces;
using TrainingProject.DomainLogic.Models.Common;
using TrainingProject.DomainLogic.Models.Users;
using TrainingProject.Web.Interfaces;

namespace TrainingProject.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ExceptionController
    {
        private IUserManager _userManager;
        private IHostServices _hostServices;

        public UsersController(IUserManager userManager, IHostServices hostServices)
        {
            _userManager = userManager;
            _hostServices = hostServices;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin,Account manager")]
        public async Task<ActionResult<Page<UserLiteDTO>>> Index([FromQuery] int index = 0, int pageSize = 20, string search = null)
        {
            return await HandleExceptions(async () =>
            {
                var role = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultRoleClaimType))?.Value;
                if (role != "Admin" && role != "Account manager")
                {
                    return Forbid("Access denied");
                }
                return Ok(await _userManager.GetUsers(index, pageSize, search));
            });
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserFullDTO>> Details(string userId)
        {
            return await HandleExceptions(async () =>
            {
                var hostRoot = _hostServices.GetHostPath();
                return Ok(await _userManager.GetUser(Guid.Parse(userId)));
            });
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] RegisterDTO registerDTO)
        {
            return await HandleExceptions(async () =>
            {
                if (ModelState.IsValid)
                {
                    await _userManager.RegisterUser(registerDTO);
                    return Ok();
                }
                return BadRequest("Model state is not valid");
            });
        }

        [HttpGet("{userId}/update")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<UserToUpdateDTO>> Update(string userId)
        {
            return await HandleExceptions(async () =>
            {
                var role = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultRoleClaimType))?.Value;
                var currentUserId = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultNameClaimType))?.Value;
                if (role != "Admin" && currentUserId != userId)
                {
                    return Forbid("Access denied");
                }
                var hostRoot = _hostServices.GetHostPath();
                return Ok(await _userManager.GetUserToUpdate(Guid.Parse(userId)));
            });
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> Update([FromForm] UserUpdateDTO userUpdateDTO)
        {
            return await HandleExceptions(async () =>
            {
                var role = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultRoleClaimType))?.Value;
                var userId = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultNameClaimType))?.Value;
                if (role != "Admin" && userId != userUpdateDTO.Id)
                {
                    return Forbid("Access denied");
                }
                if (ModelState.IsValid)
                {
                    var hostRoot = _hostServices.GetHostPath();
                    await _userManager.UpdateUser(userUpdateDTO, hostRoot);
                    return Ok();
                }
                return BadRequest("Model state is not valid");
            });
        }

        [HttpDelete("{userId}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Account manager")]
        public async Task<ActionResult> Delete(string userId)
        {
            return await HandleExceptions(async () =>
            {
                var hostRoot = _hostServices.GetHostPath();
                await _userManager.DeleteUser(Guid.Parse(userId), false, hostRoot);
                return Ok();
            });
        }

        [HttpGet("{userId}/ban")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin,Account manager")]
        public async Task<ActionResult<UserToBanDTO>> Ban(string userId)
        {
            return await HandleExceptions(async () =>
            {
                var currentRole = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultRoleClaimType))?.Value;
                var userRole = (await _userManager.GetUserRole(Guid.Parse(userId))).ToString();
                if (userRole == "Account manager" || userRole == currentRole)
                {
                    return Forbid("Lack of rights");
                }
                return Ok(await _userManager.GetUserToBan(Guid.Parse(userId)));
            });
        }

        [HttpPut]
        [Route("ban")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin,Account manager")]
        public async Task<ActionResult> Ban([FromBody]BanDTO banDTO)
        {
            return await HandleExceptions(async () =>
            {
                var currentRole = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultRoleClaimType))?.Value;
                var userRole = (await _userManager.GetUserRole(Guid.Parse(banDTO.Id))).ToString();
                if (userRole == "Account manager" || userRole == currentRole)
                {
                    return Forbid("Lack of rights");
                }
                if (ModelState.IsValid)
                {
                    await _userManager.BanUser(banDTO);
                    return Ok();
                }
                return BadRequest("Model state is not valid");
            });
        }

        [HttpPut("{userId}/unban")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin,Account manager")]
        public async Task<ActionResult> Unban(string userId)
        {
            return await HandleExceptions(async () =>
            {
                if (!Guid.TryParse(userId, out Guid userGuid))
                {
                    return BadRequest("Invalid user id");
                }
                var currentRole = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultRoleClaimType))?.Value;
                var userRole = (await _userManager.GetUserRole(userGuid)).ToString();
                if (userRole == "Account manager" || userRole == currentRole)
                {
                    return Forbid("Lack of rights");
                }
                await _userManager.UnbanUser(userGuid);
                return Ok();
            });
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Account manager")]
        [Route("roles")]
        public async Task<ActionResult<IEnumerable<Role>>> Roles()
        {
            return await HandleExceptions(async () =>
            {
                return Ok(await _userManager.GetRoles());
            });
        }

        [HttpGet("{userId}/role")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Account manager")]
        public async Task<ActionResult<UserRoleDTO>> ChangeRole(string userId)
        {
            return await HandleExceptions(async () =>
            {
                return Ok(await _userManager.GetUserWithRole(Guid.Parse(userId)));
            });
        }

        [HttpPut]
        [Route("role")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Account manager")]
        public async Task<ActionResult> ChangeRole([FromBody] ChangeRoleDTO changeRoleDTO)
        {
            return await HandleExceptions(async () =>
            {
                if (ModelState.IsValid)
                {
                    await _userManager.ChangeRole(changeRoleDTO);
                    return Ok();
                }
                return BadRequest("Model state is not valid");
            });
        }

        [HttpPost]
        [Route("signIn")]
        public async Task<ActionResult> SignIn([FromBody] LoginDTO loginDTO)
        {
            return await HandleExceptions(async () =>
            {
                if (ModelState.IsValid)
                {
                    return Ok(await _userManager.Login(loginDTO));
                }
                return BadRequest("Model state is not valid");
            });

        }

        [HttpPut]
        [Route("changePassword")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> ChangePassword([FromBody]ChangePasswordDTO changePasswordDTO)
        {
            return await HandleExceptions(async () =>
            {
                if (ModelState.IsValid)
                {
                    await _userManager.ChangePassword(changePasswordDTO);
                    return Ok();
                }
                return BadRequest("Model state is not valid");
            });
        }
    }
}
