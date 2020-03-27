using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TrainingProject.Data;
using TrainingProject.Domain;
using TrainingProject.DomainLogic.Interfaces;
using TrainingProject.DomainLogic.Models.Common;
using TrainingProject.DomainLogic.Models.Events;

namespace TrainingProject.DomainLogic.Managers
{
    public class EventManager : IEventManager
    {
        private readonly IAppContext _appContext;
        private readonly IMapper _mapper;
        public EventManager(IAppContext appContext, IMapper mapper)
        {
            _appContext = appContext;
            _mapper = mapper;
        }

        public async Task AddEvent(EventCreateDTO @event, string hostRoot)
        {
            var newEvent = _mapper.Map<EventCreateDTO, Event>(@event);
            await _appContext.Events.AddAsync(newEvent);
            await _appContext.SaveChangesAsync(default);

            if (@event.Image != null)
            {
                string path = $"/img/events/{newEvent.Id}.jpg";
                await using var fileStream = new FileStream(hostRoot + path, FileMode.Create);
                await @event.Image.CopyToAsync(fileStream);
            }

            foreach (var tagName in @event.Tags)
            {
                var tag = await _appContext.Tags.FirstOrDefaultAsync(t => t.Name.ToLower() == tagName);
                if (tag == null)
                {
                    tag = _mapper.Map<string, Tag>(tagName);
                    await _appContext.Tags.AddAsync(tag);
                    await _appContext.SaveChangesAsync(default);
                }
                await _appContext.EventsTags.AddAsync(new EventsTags {EventId = newEvent.Id, TagId = tag.Id});
            }
            await _appContext.SaveChangesAsync(default);
        }

        public Task UpdateEvent(EventUpdateDTO @event)
        {
            //вручную проверить теги и картинку
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

        public Task<EventFullDTO> GetEvent(int eventId, string hostPath)
        {
            throw new NotImplementedException();
        }

        public Task<Page<EventLiteDTO>> GetEvents(int index, int pageSize, string hostPath, string search, byte? categoryId, string tag, bool? upComing, bool onlyFree,
            bool vacancies, int? organizer, int? participant)
        {
            throw new NotImplementedException();
        }

        public async Task SignUp(int userId, int eventId)
        {
            var eu = new EventsUsers { ParticipantId = userId, EventId = eventId};
            await _appContext.EventsUsers.AddAsync(eu);
            await _appContext.SaveChangesAsync(default);
        }

        public async Task Unsubscribe(int userId, int eventId)
        {
            var eu = await _appContext.EventsUsers.FindAsync(eventId, userId);
            if (eu != null)
            {
                _appContext.EventsUsers.Remove(eu);
            }
            await _appContext.SaveChangesAsync(default);
        }
    }
}
