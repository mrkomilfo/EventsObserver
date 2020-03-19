using System.Collections.Generic;
using System.Threading.Tasks;
using TrainingProject.Domain;

namespace TrainingProject.Data.Interfaces
{
    interface IEventRepository
    {
        Task<Event> GetEvent(int id);
        Task<Page<Event>> GetEvents(int index, int pageSize, byte? categoryId, string tag, bool upComing, bool onlyFree, bool vacancies);
        Task AddEvent(Event @event);
        Task UpdateEvent(Event @event);
        Task DeleteEvent(int id);

        Task<Category> GetCategory(byte id);
        Task AddCategory(Category category);
        Task UpdateCategory(Category category);
        Task DeleteCategory(Category category);
        Task<IEnumerable<string>> GetCategories();
    }
}
