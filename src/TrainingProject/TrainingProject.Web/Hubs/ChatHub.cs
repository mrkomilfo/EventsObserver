using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrainingProject.Common;
using TrainingProject.DomainLogic.Interfaces;
using TrainingProject.Web.Filters;

namespace TrainingProject.Web.Hubs
{
    [ServiceFilter(typeof(ExceptionHandlingFilter))]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ChatHub : Hub
    {
        private readonly IUserManager _userManager;
        private readonly IEventManager _eventManager;
        private readonly ILogHelper _logger;
        public ChatHub(IUserManager userManager, IEventManager eventManager, ILogHelper logger)
        {
            _userManager = userManager;
            _eventManager = eventManager;
            _logger = logger;
        }
        public async Task Send(int eventId, string message)
        {
            _logger.LogMethodCallingWithObject(new { eventId, message });
            var userName = await _userManager.GetUserName(Guid.Parse(Context.User.Identity.Name));
            IReadOnlyList<string> usersIds = (IReadOnlyList<string>) await _eventManager.GetEventInvolvedUsersId(eventId);
            await Clients.Users(usersIds).SendAsync("Send", userName, message);
        }
    }
}
