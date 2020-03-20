using System.Collections.Generic;
using System.Threading.Tasks;
using TrainingProject.Domain;

namespace TrainingProject.Data.Interfaces
{
    public interface IEventRepository
    {
        Task<Event> GetEvent(int id);
        Task<Page<Event>> GetEvents(int index, int pageSize, string search, byte? categoryId, string tag, bool? upComing, bool onlyFree, bool vacancies, int? organizer, int? participant);
        Task<int> GetEventsNum(byte? category, int? organizer, int? participant);
        Task AddEvent(Event @event);
        Task UpdateEvent(Event @event);
        Task DeleteEvent(int id);

        Task<Category> GetCategory(byte id);
        Task AddCategory(Category category);
        Task UpdateCategory(Category category);
        Task DeleteCategory(byte category);
        Task<IEnumerable<Category>> GetCategories();
    }
}
