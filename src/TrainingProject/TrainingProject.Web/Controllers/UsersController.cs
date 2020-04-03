using CSharpFunctionalExtensions;
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
    public class UsersController : ControllerBase
    {
        private IUserManager _userManager;
        private IHostServices _hostServices;

        public UsersController(IUserManager userManager, IHostServices hostServices)
        {
            _userManager = userManager;
            _hostServices = hostServices;
        }

        [HttpGet]
        [Route("Index")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<Page<UserLiteDTO>>> Index([FromQuery] int index = 0, int pageSize = 12)
        {
            var role = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultRoleClaimType))?.Value;
            if (role != "Admin" && role != "AccountManager")
            {
                return Forbid("Access denied");
            }
            return Ok(await _userManager.GetUsers(index, pageSize));
        }

        [HttpGet("{userId}")]
        [Route("Details")]
        public async Task<ActionResult<UserFullDTO>> Details([FromQuery] string userId)
        {
            var hostRoot = _hostServices.GetHostPath();
            return await _userManager.GetUser(Guid.Parse(userId), hostRoot)
                .ToResult(NotFound($"User with id = {userId} was not found"))
                .Finally(result => result.IsSuccess ? (ActionResult)Ok(result.Value) : BadRequest(result.Error));
        }

        [HttpPost]
        [Authorize]
        [Route("Register")]
        public async Task<ActionResult> Create([FromBody] RegisterDTO registerDTO)
        {
            if (ModelState.IsValid)
            {
                await _userManager.RegisterUser(registerDTO);
                return Ok();
            }
            return BadRequest("Model state is not valid");
        }

        [HttpGet("{userId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Route("Update")]
        public async Task<ActionResult<UserToUpdateDTO>> Update(string userId)
        {
            var role = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultRoleClaimType))?.Value;
            var currentUserId = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultNameClaimType))?.Value;
            if (role != "Admin" && currentUserId != userId)
            {
                return Forbid("Access denied");
            }
            var hostRoot = _hostServices.GetHostPath();
            return await _userManager.GetUserToUpdate(Guid.Parse(userId), hostRoot)
                .ToResult(NotFound($"User with id = {userId} was not found"))
                .Finally(result => result.IsSuccess ? (ActionResult)Ok(result.Value) : BadRequest(result.Error));
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Route("Update")]
        public async Task<ActionResult> Update([FromBody] UserUpdateDTO userUpdateDTO)
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
        }

        [HttpDelete("{userId}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "AccountManager")]
        [Route("Delete")]
        public async Task<ActionResult> Delete(string userId)
        {
            var hostRoot = _hostServices.GetHostPath();
            await _userManager.DeleteUser(Guid.Parse(userId), false, hostRoot);
            return Ok();
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Route("Ban")]
        public async Task<ActionResult> Ban(BanDTO banDTO)
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
        }

        [HttpPut("{userId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Route("Unban")]
        public async Task<ActionResult> Unban(string userId)
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
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "AccountManager")]
        [Route("Roles")]
        public async Task<ActionResult<IEnumerable<Role>>> Roles()
        {
            return Ok(await _userManager.GetRoles());
        }

        [HttpGet("{userId}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "AccountManager")]
        [Route("ChangeRole")]
        public async Task<ActionResult<UserRoleDTO>> ChangeRole(string userId)
        {
            return await _userManager.GetUserWithRole(Guid.Parse(userId))
                .ToResult(NotFound($"User with id = {userId} was not found"))
                .Finally(result => result.IsSuccess ? (ActionResult)Ok(result.Value) : BadRequest(result.Error));
        }

        [HttpPut("{userId}")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "AccountManager")]
        [Route("ChangeRole")]
        public async Task<ActionResult> ChangeRole([FromBody] ChangeRoleDTO changeRoleDTO)
        {
            if (ModelState.IsValid)
            {
                await _userManager.ChangeRole(changeRoleDTO);
                return Ok();
            }
            return BadRequest("Model state is not valid");
        }
    }
}
