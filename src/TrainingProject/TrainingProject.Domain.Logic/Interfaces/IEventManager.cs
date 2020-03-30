using CSharpFunctionalExtensions;
using System.Threading.Tasks;
using TrainingProject.DomainLogic.Models.Common;
using TrainingProject.DomainLogic.Models.Events;

namespace TrainingProject.DomainLogic.Interfaces
{
    public interface IEventManager
    {
        Task AddEvent(EventCreateDTO @event, string hostRoot);
        Task UpdateEvent(EventUpdateDTO @event, string hostRoot);
        Task DeleteEvent(int eventId, bool force, string hostRoot);
        Task<Maybe<EventFullDTO>> GetEvent(int eventId, string hostRoot);
        Task<Page<EventLiteDTO>> GetEvents(int index, int pageSize, string hostPath, string search, byte? categoryId, string tag, bool? upComing, bool onlyFree, bool vacancies, int? organizer, int? participant);
        Task SignUp(int userId, int eventId);
        Task Unsubscribe(int userId, int eventId);
    }
}
