using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrainingProject.Data.Interfaces;
using TrainingProject.Domain;
using  System.Linq;
using Microsoft.EntityFrameworkCore;

namespace TrainingProject.Data.Repositories
{
    public class EventRepository : BaseRepository, IEventRepository
    {
        public EventRepository(string connectionString, IAppContextFactory contextFactory) : base(connectionString, contextFactory) { }

        public async Task<Event> GetEvent(int id)
        {
            await using var context = ContextFactory.CreateDbContext(ConnectionString);
            return await context.Events.Include(e => e.Category).Include(e => e.Tags).Include(e => e.Organizer).Include(e => e.Participants).FirstOrDefaultAsync(e => e.EventId == id);
        }

        public async Task<Page<Event>> GetEvents(int index, int pageSize, string search, byte? categoryId, string tag, bool? upComing, bool onlyFree,
            bool vacancies, int? organizer, int? participant)
        {
            var result = new Page<Event>() { CurrentPage = index, PageSize = pageSize };
            await using var context = ContextFactory.CreateDbContext(ConnectionString);
            var query = context.Events.Include(e => e.Category).Include(e=>e.Tags).Include(e=>e.Organizer).Include(e=>e.Participants).AsQueryable();
            if (search != null)
            {
                query = query.Where(e=>e.Name.Contains(search, StringComparison.CurrentCultureIgnoreCase));
            }
            if (categoryId != null)
            {
                query = query.Where(e => e.Category.CategoryId == categoryId);
            }
            if (tag != null)
            {
                query = query.Where(e => e.Tags.Any(t=>t.Name == tag));
            }
            if (upComing != null)
            {
                query = query.Where(e => (bool)upComing ? e.Start >= DateTime.Now : e.Start < DateTime.Now);
            }
            if (onlyFree)
            {
                query = query.Where(e => e.Fee == 0);
            }
            if (vacancies)
            {
                query = query.Where(e => e.ParticipantsLimit == 0 || e.Participants.Count() < e.ParticipantsLimit);
            }
            if (organizer != null)
            {
                query = query.Where(e => e.Organizer.UserId == organizer);
            }
            if (participant != null)
            {
                query = query.Where(e => e.Participants.Any(p=>p.UserId == participant));
            }

            if (upComing != null && (bool)upComing)
            {
                query = query.OrderBy(e => e.Start).Skip(index * pageSize).Take(pageSize);
            }
            else
            {
                query = query.OrderByDescending(e => e.Start).Skip(index * pageSize).Take(pageSize);
            }
            result.TotalRecords = await query.CountAsync();
            result.Records = await query.ToListAsync();

            return result;
        }

        public async Task<int> GetEventsNum(byte? category, int? organizer, int? participant)
        {
            await using var context = ContextFactory.CreateDbContext(ConnectionString);
            var query = context.Events.Include(e => e.Category).Include(e => e.Organizer).Include(e => e.Participants).AsQueryable();
            if (category != null)
            {
                return query.Count(e => e.Category.CategoryId == category);
            }
            if (organizer != null)
            {
                return query.Count(e => e.Organizer.UserId == organizer);
            }
            if (participant != null)
            {
                return query.Count(e => e.Participants.Any(p=>p.UserId == participant));
            }
            return query.Count();
        }

        public async Task AddEvent(Event @event)
        {
            await using var context = ContextFactory.CreateDbContext(ConnectionString);
            await context.Events.AddAsync(@event);
            await context.SaveChangesAsync();
        }

        public async Task UpdateEvent(Event @event)
        {
            await using var context = ContextFactory.CreateDbContext(ConnectionString);
            context.Entry(@event).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }

        public async Task DeleteEvent(int id)
        {
            await using var context = ContextFactory.CreateDbContext(ConnectionString);
            var eventToDelete = new Event() { EventId = id};
            context.Events.Remove(eventToDelete);
            await context.SaveChangesAsync();
        }

        public async Task<Category> GetCategory(byte id)
        {
            await using var context = ContextFactory.CreateDbContext(ConnectionString);
            return await context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
        }

        public async Task AddCategory(Category category)
        {
            await using var context = ContextFactory.CreateDbContext(ConnectionString);
            await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();
        }

        public async Task UpdateCategory(Category category)
        {
            await using var context = ContextFactory.CreateDbContext(ConnectionString);
            context.Entry(category).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }

        public async Task DeleteCategory(byte id)
        {
            await using var context = ContextFactory.CreateDbContext(ConnectionString);
            var category = new Category() { CategoryId = id };
            context.Categories.Remove(category);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Category>> GetCategories()
        {
            await using var context = ContextFactory.CreateDbContext(ConnectionString);
            return context.Categories.AsEnumerable();
        }
    }
}
