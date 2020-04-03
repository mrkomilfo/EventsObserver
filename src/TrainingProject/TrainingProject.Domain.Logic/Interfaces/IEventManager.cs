using CSharpFunctionalExtensions;
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
        Task<Maybe<EventToUpdateDTO>> GetEventToUpdate(int eventId, string hostRoot);
        Task DeleteEvent(int eventId, bool force, string hostRoot);
        Task<Maybe<EventFullDTO>> GetEvent(int eventId, string hostRoot);
        Task<Page<EventLiteDTO>> GetEvents(int index, int pageSize, string hostPath, string search, byte? categoryId, string tag, bool? upComing, bool onlyFree, bool vacancies, Guid? organizer, Guid? participant);
        Task SignUp(Guid userId, int eventId);
        Task Unsubscribe(Guid userId, int eventId);
        Task<Guid?> GetEventOrganizerId(int eventId);
    }
}
