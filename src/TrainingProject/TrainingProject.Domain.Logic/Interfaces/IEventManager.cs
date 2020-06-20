using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrainingProject.DomainLogic.Models.Common;
using TrainingProject.DomainLogic.Models.Events;

namespace TrainingProject.DomainLogic.Interfaces
{
    public interface IEventManager
    {
        Task AddEventAsync(EventCreateDTO @event, string hostRoot);
        Task UpdateEventAsync(EventUpdateDTO @event, string hostRoot);
        Task<EventToUpdateDTO> GetEventToUpdateAsync(int eventId);
        Task DeleteEventAsync(int eventId, bool force, string hostRoot);
        Task<EventFullDTO> GetEventAsync(int eventId);
        Task<Page<EventLiteDTO>> GetEventsAsync(int index, int pageSize, string search, int? categoryId, string tag, bool? upComing, bool onlyFree, bool vacancies, Guid organizerId, Guid participantId);
        Task SubscribeAsync(Guid userId, int eventId);
        Task UnsubscribeAsync(Guid userId, int eventId);
        Task<Guid?> GetEventOrganizerIdAsync(int eventId);
        Task<IList<string>> GetEventInvolvedUsersIdAsync(int eventId);
        void Notificate();
    }
}
