using System;
using System.Threading.Tasks;
using TrainingProject.DomainLogic.Models.Common;
using TrainingProject.DomainLogic.Models.Events;

namespace TrainingProject.DomainLogic.Interfaces
{
    public interface IEventManager
    {
        Task AddEvent(EventCreateDTO @event, string hostRoot);
        Task UpdateEvent(EventUpdateDTO @event, string hostRoot);
        Task<EventToUpdateDTO> GetEventToUpdate(int eventId);
        Task DeleteEvent(int eventId, bool force, string hostRoot);
        Task<EventFullDTO> GetEvent(int eventId);
        Task<Page<EventLiteDTO>> GetEvents(int index, int pageSize, string search, int? categoryId, string tag, bool? upComing, bool onlyFree, bool vacancies, Guid organizer, Guid participant);
        Task Subscribe(Guid userId, int eventId);
        Task Unsubscribe(Guid userId, int eventId);
        Task<Guid?> GetEventOrganizerId(int eventId);
        void Notificate();
    }
}
