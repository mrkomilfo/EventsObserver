using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TrainingProject.Data;
using TrainingProject.Domain;
using TrainingProject.DomainLogic.Interfaces;
using TrainingProject.DomainLogic.Models.Common;
using TrainingProject.DomainLogic.Models.Events;
using Microsoft.AspNetCore.Hosting;

namespace TrainingProject.DomainLogic.Managers
{
    public class EventManager : IEventManager
    {
        private readonly IAppContext _appContext;
        private readonly IWebHostEnvironment _environment;
        public EventManager(IAppContext appContext, IWebHostEnvironment environment)
        {
            _appContext = appContext;
            _environment = environment;
        }

        public async Task AddEvent(EventCreateDTO @event)
        {
            Event newEvent = new Event()
            {
                Name = @event.Name,
                CategoryId = @event.CategoryId,
                Description = @event.Description,
                Start = @event.Start,
                Place = @event.Place,
                Fee = @event.Fee,
                ParticipantsLimit = @event.ParticipantsLimit,
                OrganizerId = @event.OrganizerId,
                HasPhoto = @event.Image != null,
                PublicationTime = DateTime.Now
            };
            await _appContext.Events.AddAsync(newEvent);
            await _appContext.SaveChangesAsync(default);

            if (@event.Image != null)
            {
                string path = "/img/" + newEvent.Id + ".jpg";
                using (var fileStream = new FileStream(_environment.WebRootPath + path, FileMode.Create))
                {
                    await @event.Image.CopyToAsync(fileStream);
                }
            }

            foreach (var tagName in @event.Tags)
            {
                var tag = await _appContext.Tags.FirstOrDefaultAsync(t => t.Name.ToLower() == tagName);
                if (tag == null)
                {
                    tag = new Tag{Name = tagName};
                    await _appContext.Tags.AddAsync(tag);
                    await _appContext.SaveChangesAsync(default);
                }
                await _appContext.EventsTags.AddAsync(new EventsTags {EventId = newEvent.Id, TagId = tag.Id});
            }
            await _appContext.SaveChangesAsync(default);
        }

        public Task UpdateEvent(EventUpdateDTO @event)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteEvent(int eventId, bool force)
        {
            var @event = await _appContext.Events.IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Id == eventId);
            if (@event != null)
            {
                if (force)
                {
                    _appContext.Events.Remove(@event);
                }
                else
                {
                    @event.IsDeleted = true;
                }
                await _appContext.SaveChangesAsync(default);
            }
        }

        public Task<EventFullDto> GetEvent(int eventId)
        {
            throw new NotImplementedException();
        }

        public Task<Page<EventLiteDTO>> GetEvents(int index, int pageSize, string search, byte? categoryId, string tag, bool? upComing, bool onlyFree,
            bool vacancies, int? organizer, int? participant)
        {
            throw new NotImplementedException();
        }

        public Task SignUp(int userId, int eventId)
        {
            throw new NotImplementedException();
        }

        public Task Unsubscribe(int userId, int eventId)
        {
            throw new NotImplementedException();
        }
    }
}
