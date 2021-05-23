using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TrainingProject.Common;
using TrainingProject.DomainLogic.Interfaces;
using TrainingProject.DomainLogic.Models.Users;
using TrainingProject.Web.Filters;

namespace TrainingProject.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(ExceptionHandlingFilter))]
    public class RecurrentEventsController : ControllerBase
    {
        private readonly IEventManager _eventManager;
        private readonly ILogHelper _logger;

        public RecurrentEventsController(IEventManager eventManager, ILogHelper logger)
        {
            _eventManager = eventManager;
            _logger = logger;
        }

        [HttpPut("{eventWeekDayId:int}/subscribe")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> SubscribeAsync(int eventWeekDayId)
        {
            _logger.LogMethodCallingWithObject(new {eventWeekDayId});

            var userId = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultNameClaimType))?.Value;
 
            await _eventManager.SubscribeOnRecurrentEventAsync(Guid.Parse(userId), eventWeekDayId);

            return Ok();
        }

        [HttpPut("{eventWeekDayId:int}/unsubscribe")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> UnsubscribeAsync(int eventWeekDayId)
        {
            _logger.LogMethodCallingWithObject(new {eventWeekDayId});

            var userId = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimsIdentity.DefaultNameClaimType))?.Value;

            await _eventManager.UnsubscribeFromRecurrentEventAsync(Guid.Parse(userId), eventWeekDayId);

            return Ok();
        }

        [HttpGet("{eventWeekDayId:int}/participants")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<ParticipantDto>> GetParticipants(int eventWeekDayId)
        {
            _logger.LogMethodCallingWithObject(new { eventWeekDayId });

            var participants = await _eventManager.GetRecurrentEventParticipants(eventWeekDayId);

            return Ok(participants);
        }

        [HttpPatch("checkParticipant/{eventParticipantId:int}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<ParticipantDto>> ToggleParticipantCheck(int eventParticipantId)
        {
            _logger.LogMethodCallingWithObject(new { eventParticipantId });

            await _eventManager.ToggleRecurrentEventParticipantCheck(eventParticipantId);

            return Ok();
        }
    }
}