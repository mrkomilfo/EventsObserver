using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TrainingProject.Common;
using TrainingProject.DomainLogic.Interfaces;
using TrainingProject.DomainLogic.Models.Common;
using TrainingProject.DomainLogic.Models.Events;
using TrainingProject.Web.Filters;
using TrainingProject.Web.Interfaces;

namespace TrainingProject.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(ExceptionHandlingFilter))]
    public class EventsController : ControllerBase
    {
        private IEventManager _eventManager;
        private IHostServices _hostServices;
        private ILogHelper _logger;

        public EventsController(IEventManager eventManager, IHostServices hostServices, ILogHelper logger)
        {
            _eventManager = eventManager;
            _hostServices = hostServices;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<Page<EventLiteDTO>>> Index([FromQuery] int page = 0, int pageSize = 4, string search = null, 
            int? categoryId = null, string tag = null, bool? upComing = null, bool onlyFree = false,
            bool vacancies = false, string organizerId = null, string participantId = null)
        {
            _logger.LogMethodCallingWithObject(new { page, pageSize, search, categoryId, tag, upComing, onlyFree, vacancies, organizerId, participantId });
            Guid.TryParse(organizerId, out var organizerGuid);
            Guid.TryParse(participantId, out var participantGuid);
            return Ok(await _eventManager.GetEventsAsync(page, pageSize, search, categoryId, tag, upComing,
                onlyFree, vacancies, organizerGuid, participantGuid));
        }

        [HttpGet("{eventId}")]
        public async Task<ActionResult<EventFullDTO>> DetailsAsync(int eventId)
        {
            _logger.LogMethodCalling();
            return Ok(await _eventManager.GetEventAsync(eventId));
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ModelStateValidation]
        public async Task<ActionResult> CreateAsync([FromForm] EventCreateDTO eventCreateDTO)
        {
            _logger.LogMethodCallingWithObject(eventCreateDTO);
            var hostRoot = _hostServices.GetHostPath();
            await _eventManager.AddEventAsync(eventCreateDTO, hostRoot);
            return Ok();
        }

        [HttpGet("{eventId}/update")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<EventToUpdateDTO>> UpdateAsync(int eventId)
        {
            _logger.LogMethodCallingWithObject(new { eventId });
            var role = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultRoleClaimType))?.Value;
            var userId = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultNameClaimType))?.Value;
            var organizerId = await _eventManager.GetEventOrganizerIdAsync(eventId);
            if (role != "Admin" && !Equals(Guid.Parse(userId), organizerId))
            {
                return Forbid("Access denied");
            }
            return Ok(await _eventManager.GetEventToUpdateAsync(eventId));
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ModelStateValidation]
        public async Task<ActionResult> UpdateAsync([FromForm] EventUpdateDTO eventUpdateDTO)
        {
            _logger.LogMethodCallingWithObject(eventUpdateDTO);
            var role = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultRoleClaimType))?.Value;
            var userId = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultNameClaimType))?.Value;
            var organizerId = await _eventManager.GetEventOrganizerIdAsync(eventUpdateDTO.Id);
            if (role != "Admin" && Guid.Parse(userId) != organizerId)
            {
                return Forbid("Access denied");
            }
            var hostRoot = _hostServices.GetHostPath();
            await _eventManager.UpdateEventAsync(eventUpdateDTO, hostRoot);
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
            if (role != "Admin" && !Equals(Guid.Parse(userId), organizerId))
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
    }
}
