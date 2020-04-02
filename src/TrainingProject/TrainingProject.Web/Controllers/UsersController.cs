using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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
        [Route("Update")]
        public async Task<ActionResult> Delete(string userId)
        {
            var hostRoot = _hostServices.GetHostPath();
            await _userManager.DeleteUser(Guid.Parse(userId), false, hostRoot);
            return Ok();
        }
    }
}
