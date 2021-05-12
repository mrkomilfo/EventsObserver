using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrainingProject.DomainLogic.Models.Common;
using TrainingProject.DomainLogic.Models.Events;

namespace TrainingProject.DomainLogic.Interfaces
{
    public interface IEventManager
    {
        Task AddEventAsync(EventCreateDto eventDto, string hostRoot);
        Task UpdateEventAsync(EventUpdateDto eventDto, string hostRoot);
        Task<EventToUpdateDto> GetEventToUpdateAsync(int eventId);
        Task DeleteEventAsync(int eventId, bool force, string hostRoot);
        Task<EventFullDto> GetEventAsync(int eventId);
        Task<Page<EventLiteDto>> GetEventsAsync(int index, int pageSize, string search, int? categoryId, string tag, bool? upComing, bool onlyFree, bool vacancies, string organizerId, string participantId);
        Task SubscribeAsync(Guid userId, int eventId);
        Task UnsubscribeAsync(Guid userId, int eventId);
        Task<Guid?> GetEventOrganizerIdAsync(int eventId);
        Task<IList<string>> GetEventInvolvedUsersIdAsync(int eventId);
        Task CheckUserInvolvementInTheEventAsync(string userId, int eventId);
        void Notify();
    }
}
