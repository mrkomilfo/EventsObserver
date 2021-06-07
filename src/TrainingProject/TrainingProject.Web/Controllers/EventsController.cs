using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using TrainingProject.Common;
using TrainingProject.DomainLogic.Interfaces;
using TrainingProject.DomainLogic.Models.Common;
using TrainingProject.DomainLogic.Models.Events;
using TrainingProject.DomainLogic.Models.Users;
using TrainingProject.Web.Filters;
using TrainingProject.Web.Interfaces;

namespace TrainingProject.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(ExceptionHandlingFilter))]
    public class EventsController : ControllerBase
    {
        private readonly IEventManager _eventManager;
        private readonly IHostServices _hostServices;
        private readonly ILogHelper _logger;

        public EventsController(IEventManager eventManager, IHostServices hostServices, ILogHelper logger)
        {
            _eventManager = eventManager;
            _hostServices = hostServices;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<Page<EventLiteDto>>> Index([FromQuery] int page = 0, [FromQuery] int pageSize = 4,
            [FromQuery] string search = null, [FromQuery] int? categoryId = null, [FromQuery] string tag = null,
            [FromQuery] string from = null, [FromQuery] string to = null, [FromQuery] string organizerId = null,
            [FromQuery] string participantId = null)
        {
            _logger.LogMethodCallingWithObject(new { page, pageSize, search, categoryId, tag, from, to, organizerId, participantId });
            return Ok(await _eventManager.GetEventsAsync(page, pageSize, search, categoryId, tag, from, to, organizerId, participantId));
        }

        [HttpGet("{eventId}")]
        public async Task<ActionResult<EventFullDto>> DetailsAsync(int eventId)
        {
            _logger.LogMethodCalling();

            var userId = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultNameClaimType))?.Value;

            return Ok(await _eventManager.GetEventAsync(eventId, userId));
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ModelStateValidation]
        public async Task<ActionResult> CreateAsync([FromForm] EventCreateDto eventCreateDto)
        {
            _logger.LogMethodCallingWithObject(eventCreateDto);

            var hostRoot = _hostServices.GetHostPath();

            await _eventManager.AddEventAsync(eventCreateDto, hostRoot);

            return Ok();
        }

        [HttpGet("{eventId}/update")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<EventToUpdateDto>> UpdateAsync(int eventId)
        {
            _logger.LogMethodCallingWithObject(new { eventId });

            var role = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultRoleClaimType))?.Value;
            var userId = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultNameClaimType))?.Value;
            var organizerId = await _eventManager.GetEventOrganizerIdAsync(eventId);

            if (role != "Admin" && !Equals(userId, organizerId.ToString()))
            {
                return Forbid("Access denied");
            }

            var eventToUpdate = await _eventManager.GetEventToUpdateAsync(eventId);

            return Ok(eventToUpdate);
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ModelStateValidation]
        public async Task<ActionResult> UpdateAsync([FromForm] EventUpdateDto eventUpdateDto)
        {
            _logger.LogMethodCallingWithObject(eventUpdateDto);

            var role = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultRoleClaimType))?.Value;
            var userId = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultNameClaimType))?.Value;
            var organizerId = await _eventManager.GetEventOrganizerIdAsync(eventUpdateDto.Id);

            if (role != "Admin" && !Equals(userId, organizerId.ToString()))
            {
                return Forbid("Access denied");
            }

            var hostRoot = _hostServices.GetHostPath();

            await _eventManager.UpdateEventAsync(eventUpdateDto, hostRoot);

            return Ok();
        }

        [HttpDelete("{eventId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> DeleteAsync(int eventId)
        {
            _logger.LogMethodCallingWithObject(new { eventId });

            var role = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultRoleClaimType))?.Value;
            var userId = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultNameClaimType))?.Value;
            var organizerId = await _eventManager.GetEventOrganizerIdAsync(eventId);

            if (role != "Admin" && !Equals(userId, organizerId.ToString()))
            {
                return Forbid("Access denied");
            }

            var hostRoot = _hostServices.GetHostPath();

            await _eventManager.DeleteEventAsync(eventId, false, hostRoot);

            return Ok();
        }

        [HttpPut("{eventId}/subscribe")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> SubscribeAsync(int eventId)
        {
            _logger.LogMethodCallingWithObject(new { eventId });

            var userId = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultNameClaimType))?.Value;

            await _eventManager.SubscribeAsync(Guid.Parse(userId), eventId);

            return Ok();
        }

        [HttpPut("{eventId}/unsubscribe")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> UnsubscribeAsync(int eventId)
        {
            _logger.LogMethodCallingWithObject(new { eventId });

            var userId = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultNameClaimType))?.Value;

            await _eventManager.UnsubscribeAsync(Guid.Parse(userId), eventId);

            return Ok();
        }

        [HttpGet("{eventId:int}/participants")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<IEnumerable<ParticipantDto>>> GetParticipants(int eventId)
        {
            _logger.LogMethodCallingWithObject(new { eventId });

            var participants = await _eventManager.GetEventParticipants(eventId);

            return Ok(participants);
        }

        [HttpPatch("{eventId:int}/approve")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ParticipantDto>>> ToggleEventApprove(int eventId)
        {
            _logger.LogMethodCallingWithObject(new { eventId });

            await _eventManager.ToggleEventApprove(eventId);

            return Ok();
        }

        [HttpPatch("checkParticipant/{eventParticipantId:int}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<ParticipantDto>> ToggleParticipantCheck(int eventParticipantId)
        {
            _logger.LogMethodCallingWithObject(new { eventParticipantId });

            await _eventManager.ToggleParticipantCheck(eventParticipantId);

            return Ok();
        }
    }
}
