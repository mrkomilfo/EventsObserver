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
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<Page<UserLiteDTO>>> Index([FromQuery] int index = 0, int pageSize = 12)
        {
            return await HandleExceptions(async () =>
            {
                var role = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultRoleClaimType))?.Value;
                if (role != "Admin" && role != "AccountManager")
                {
                    return Forbid("Access denied");
                }
                return Ok(await _userManager.GetUsers(index, pageSize));
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
        [Authorize]
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
        public async Task<ActionResult> Update([FromBody] UserUpdateDTO userUpdateDTO)
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
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "AccountManager")]
        public async Task<ActionResult> Delete(string userId)
        {
            return await HandleExceptions(async () =>
            {
                var hostRoot = _hostServices.GetHostPath();
                await _userManager.DeleteUser(Guid.Parse(userId), false, hostRoot);
                return Ok();
            });
        }

        [HttpPut("ban")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> Ban([FromBody] BanDTO banDTO)
        {
            return await HandleExceptions(async () =>
            {
                var currentRole = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultRoleClaimType))?.Value;
                var userRole = (await _userManager.GetUserRole(Guid.Parse(banDTO.UserId))).ToString();
                if (userRole == "AccountManager" || userRole == currentRole)
                {
                    return Forbid("Access denied");
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
        [Authorize(AuthenticationSchemes = "Bearer")]
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
                if (userRole == "AccountManager" || userRole == currentRole)
                {
                    return Forbid("Access denied");
                }
                await _userManager.UnbanUser(userGuid);
                return Ok();
            });
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "AccountManager")]
        [Route("roles")]
        public async Task<ActionResult<IEnumerable<Role>>> Roles()
        {
            return await HandleExceptions(async () =>
            {
                return Ok(await _userManager.GetRoles());
            });
        }

        [HttpGet("{userId}/role")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "AccountManager")]
        public async Task<ActionResult<UserRoleDTO>> ChangeRole(string userId)
        {
            return await HandleExceptions(async () =>
            {
                return Ok(await _userManager.GetUserWithRole(Guid.Parse(userId)));
            });
        }

        [HttpPut("role")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "AccountManager")]
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

    }
}
