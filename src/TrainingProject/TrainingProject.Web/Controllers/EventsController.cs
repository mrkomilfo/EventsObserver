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
using TrainingProject.Web.Interfaces;

namespace TrainingProject.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ExceptionController
    {
        private IEventManager _eventManager;
        private IHostServices _hostServices;
        private ILogHelper _logger;

        public EventsController(IEventManager eventManager, IHostServices hostServices, ILogHelper logger) : base(logger)
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
            return await HandleExceptions(async () =>
            {
                Guid.TryParse(organizerId, out var organizerGuid);
                Guid.TryParse(participantId, out var participantGuid);         
                return Ok(await _eventManager.GetEvents(page, pageSize, search, categoryId, tag, upComing, onlyFree, vacancies, organizerGuid, participantGuid));
            });
        }

        [HttpGet("{eventId}")]
        public async Task<ActionResult<EventFullDTO>> Details(int eventId)
        {
            _logger.LogMethodCalling();
            return await HandleExceptions(async () => Ok(await _eventManager.GetEvent(eventId)));
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> Create([FromForm] EventCreateDTO eventCreateDTO)
        {
            _logger.LogMethodCallingWithObject(eventCreateDTO);
            return await HandleExceptions(async () =>
            {
                if (ModelState.IsValid)
                {
                    var hostRoot = _hostServices.GetHostPath();
                    await _eventManager.AddEvent(eventCreateDTO, hostRoot);
                    return Ok();
                }
                return BadRequest("Model state is not valid");
            });
        }

        [HttpGet("{eventId}/update")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<EventToUpdateDTO>> Update(int eventId)
        {
            _logger.LogMethodCallingWithObject(new { eventId });
            return await HandleExceptions(async () =>
            {
                var role = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultRoleClaimType))?.Value;
                var userId = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultNameClaimType))?.Value;
                var organizerId = await _eventManager.GetEventOrganizerId(eventId);
                if (role != "Admin" && !Equals(Guid.Parse(userId), organizerId))
                {
                    return Forbid("Access denied");
                }
                return Ok(await _eventManager.GetEventToUpdate(eventId));
            });
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> Update([FromForm] EventUpdateDTO eventUpdateDTO)
        {
            _logger.LogMethodCallingWithObject(eventUpdateDTO);
            return await HandleExceptions(async () =>
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
            });
        }

        [HttpDelete("{eventId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> Delete(int eventId)
        {
            _logger.LogMethodCallingWithObject(new { eventId });
            return await HandleExceptions(async () =>
            {
                var role = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultRoleClaimType))?.Value;
                var userId = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultNameClaimType))?.Value;
                var organizerId = await _eventManager.GetEventOrganizerId(eventId);
                if (role != "Admin" && !Equals(Guid.Parse(userId), organizerId))
                {
                    return Forbid("Access denied");
                }
                var hostRoot = _hostServices.GetHostPath();
                await _eventManager.DeleteEvent(eventId, false, hostRoot);
                return Ok();
            });
        }

        [HttpPut("{eventId}/subscribe")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> Subscribe(int eventId)
        {
            _logger.LogMethodCallingWithObject(new { eventId });
            return await HandleExceptions(async () =>
            {
                var userId = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultNameClaimType))?.Value;
                await _eventManager.Subscribe(Guid.Parse(userId), eventId);
                return Ok();
            });
        }

        [HttpPut("{eventId}/unsubscribe")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> Unsubscribe(int eventId)
        {
            _logger.LogMethodCallingWithObject(new { eventId });
            return await HandleExceptions(async () =>
            {
                var userId = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultNameClaimType))?.Value;
                await _eventManager.Unsubscribe(Guid.Parse(userId), eventId);
                return Ok();
            });
        }
    }
}
