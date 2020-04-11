using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TrainingProject.DomainLogic.Interfaces;
using TrainingProject.DomainLogic.Models.Common;
using TrainingProject.DomainLogic.Models.Events;
using TrainingProject.Web.Interfaces;

namespace TrainingProject.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private IEventManager _eventManager;
        private IHostServices _hostServices;

        public EventsController(IEventManager eventManager, IHostServices hostServices)
        {
            _eventManager = eventManager;
            _hostServices = hostServices;
        }

        [HttpGet]
        [Route("Index")]
        public async Task<ActionResult<Page<EventLiteDTO>>> Index([FromQuery] int page = 0, int pageSize = 12, string search = null, int? categoryId = null, string tag = null, bool? upComing = true, bool onlyFree = false,
            bool vacancies = false, string organizer = null, string participant = null)
        {
            Guid? organizerGuid;
            if (Guid.TryParse(organizer, out _)) {
                organizerGuid = Guid.Parse(organizer);
            }
            else {
                organizerGuid = null;
            }
            Guid? participantGuid;
            if (Guid.TryParse(participant, out _))
            {
                participantGuid = Guid.Parse(participant);
            }
            else
            {
                participantGuid = null;
            }
            var hostRoot = _hostServices.GetHostPath();
            return Ok(await _eventManager.GetEvents(page, pageSize, hostRoot, search, categoryId, tag, upComing, onlyFree, vacancies, organizerGuid, participantGuid));
        }

        [HttpGet("{eventId}")]
        [Route("Details")]
        public async Task<ActionResult<EventFullDTO>> Details([FromQuery] int eventId)
        {
            var hostRoot = _hostServices.GetHostPath();
            return await _eventManager.GetEvent(eventId, hostRoot)
                .ToResult(NotFound($"Event with id = {eventId} was not found"))
                .Finally(result => result.IsSuccess ? (ActionResult)Ok(result.Value) : BadRequest(result.Error));
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Route("Create")]
        public async Task<ActionResult> Create([FromBody] EventCreateDTO eventCreateDTO)
        {
            if (ModelState.IsValid)
            {
                var hostRoot = _hostServices.GetHostPath();
                await _eventManager.AddEvent(eventCreateDTO, hostRoot);
                return Ok();
            }
            return BadRequest("Model state is not valid");
        }

        [HttpGet("{eventId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Route("Update")]
        public async Task<ActionResult<EventToUpdateDTO>> Update(int eventId)
        {
            var role = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultRoleClaimType))?.Value;
            var userId = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultNameClaimType))?.Value;
            var organizerId = await _eventManager.GetEventOrganizerId(eventId);
            if (role != "Admin" && Guid.Parse(userId) != organizerId)
            {
                return Forbid("Access denied");
            }
            var hostRoot = _hostServices.GetHostPath();
            return await _eventManager.GetEventToUpdate(eventId, hostRoot)
                .ToResult(NotFound($"Event with id = {eventId} was not found"))
                .Finally(result => result.IsSuccess ? (ActionResult)Ok(result.Value) : BadRequest(result.Error));
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Route("Update")]
        public async Task<ActionResult> Update([FromBody] EventUpdateDTO eventUpdateDTO)
        {
            var role = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultRoleClaimType))?.Value;
            var userId = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultNameClaimType))?.Value;
            var organizerId = await _eventManager.GetEventOrganizerId(eventUpdateDTO.Id);
            if (role != "Admin" && Guid.Parse(userId) != organizerId)
            {
                return Forbid("Access denied");
            }
            if (ModelState.IsValid)
            {
                var hostRoot = _hostServices.GetHostPath();
                await _eventManager.UpdateEvent(eventUpdateDTO, hostRoot);
                return Ok();
            }
            return BadRequest("Model state is not valid");
        }

        [HttpDelete("{eventId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Route("Delete")]
        public async Task<ActionResult> Delete(int eventId)
        {
            var role = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultRoleClaimType))?.Value;
            var userId = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultNameClaimType))?.Value;
            var organizerId = await _eventManager.GetEventOrganizerId(eventId);
            if (role != "Admin" && Guid.Parse(userId) != organizerId)
            {
                return Forbid("Access denied");
            }
            var hostRoot = _hostServices.GetHostPath();
            await _eventManager.DeleteEvent(eventId, false, hostRoot);
            return Ok();
        }
    }
}
