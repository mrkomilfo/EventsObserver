using System.Threading.Tasks;
using TrainingProject.DomainLogic.Models.Common;
using TrainingProject.DomainLogic.Models.Events;

namespace TrainingProject.DomainLogic.Interfaces
{
    public interface IEventManager
    {
        Task AddEvent(EventCreateDTO @event);
        Task UpdateEvent(EventUpdateDTO @event);
        Task DeleteEvent(int eventId, bool force);
        Task<EventFullDto> GetEvent(int eventId);
        Task<Page<EventLiteDTO>> GetEvents(int index, int pageSize, string search, byte? categoryId, string tag, bool? upComing, bool onlyFree, bool vacancies, int? organizer, int? participant);
        Task SignUp(int userId, int eventId);
        Task Unsubscribe(int userId, int eventId);
    }
}
