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
    public class EventsController : ExceptionController
    {
        private IEventManager _eventManager;
        private IHostServices _hostServices;

        public EventsController(IEventManager eventManager, IHostServices hostServices)
        {
            _eventManager = eventManager;
            _hostServices = hostServices;
        }

        [HttpGet]
        public async Task<ActionResult<Page<EventLiteDTO>>> Index([FromQuery] int page = 0, int pageSize = 2, string search = null, int? categoryId = null, string tag = null, bool? upComing = null, bool onlyFree = false,
            bool vacancies = false, string organizer = null, string participant = null)
        {
            return await HandleExceptions(async () =>
            {
                Guid? organizerGuid;
                if (Guid.TryParse(organizer, out _))
                {
                    organizerGuid = Guid.Parse(organizer);
                }
                else
                {
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
                return Ok(await _eventManager.GetEvents(page, pageSize, search, categoryId, tag, upComing, onlyFree, vacancies, organizerGuid, participantGuid));
            });
        }

        [HttpGet("{eventId}")]
        public async Task<ActionResult<EventFullDTO>> Details(int eventId)
        {
            return await HandleExceptions(async () =>
            {
                return Ok(await _eventManager.GetEvent(eventId));
            });
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> Create([FromForm] EventCreateDTO eventCreateDTO)
        {
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
            return await HandleExceptions(async () =>
            {
                var role = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultRoleClaimType))?.Value;
                var userId = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultNameClaimType))?.Value;
                var organizerId = await _eventManager.GetEventOrganizerId(eventId);
                if (role != "Admin" && Guid.Parse(userId) != organizerId)
                {
                    return Forbid("Access denied");
                }
                var hostRoot = _hostServices.GetHostPath();
                return Ok(await _eventManager.GetEventToUpdate(eventId));
            });
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> Update([FromBody] EventUpdateDTO eventUpdateDTO)
        {
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
            return await HandleExceptions(async () =>
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
            });
        }

        [HttpPut("{eventId}/subscribe")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> Subscribe(int eventId)
        {
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
            return await HandleExceptions(async () =>
            {
                var userId = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultNameClaimType))?.Value;
                await _eventManager.Unsubscribe(Guid.Parse(userId), eventId);
                return Ok();
            });
        }
    }
}
