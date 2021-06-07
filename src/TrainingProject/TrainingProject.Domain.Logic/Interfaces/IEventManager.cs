using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using TrainingProject.DomainLogic.Models.Common;
using TrainingProject.DomainLogic.Models.Events;
using TrainingProject.DomainLogic.Models.Users;

namespace TrainingProject.DomainLogic.Interfaces
{
    public interface IEventManager
    {
        Task AddEventAsync(EventCreateDto eventDto, string hostRoot);

        Task UpdateEventAsync(EventUpdateDto eventDto, string hostRoot);

        Task<EventToUpdateDto> GetEventToUpdateAsync(int eventId);

        Task DeleteEventAsync(int eventId, bool force, string hostRoot);

        Task<EventFullDto> GetEventAsync(int eventId, string userId=null);

        Task<Page<EventLiteDto>> GetEventsAsync(int index, int pageSize, string search, int? categoryId, string tag,
            string from, string to, string organizerId, string participantId);

        Task SubscribeAsync(Guid userId, int eventId);

        Task UnsubscribeAsync(Guid userId, int eventId);

        Task<Guid?> GetEventOrganizerIdAsync(int eventId);

        void Notify();

        Task SubscribeOnRecurrentEventAsync(Guid userId, int eventDayOfWeekId);

        Task UnsubscribeFromRecurrentEventAsync(Guid userId, int eventDayOfWeekId);

        Task<IEnumerable<ParticipantDto>> GetEventParticipants(int eventId);

        Task<IEnumerable<ParticipantDto>> GetRecurrentEventParticipants(int eventDayOfWeekId);

        Task ToggleEventApprove(int eventId);

        Task ToggleParticipantCheck(int id);

        Task ToggleRecurrentEventParticipantCheck(int id);
    }
}
