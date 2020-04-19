using System;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TrainingProject.Data;
using TrainingProject.Domain;
using TrainingProject.DomainLogic.Interfaces;
using TrainingProject.DomainLogic.Models.Common;
using TrainingProject.DomainLogic.Models.Events;
using System.Linq;

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
                string path = $"{hostRoot}\\wwroot\\img\\events\\{newEvent.Id}.jpg";
                await using var fileStream = new FileStream(path, FileMode.Create);
                await @event.Image.CopyToAsync(fileStream);
            }

            foreach (var tagName in @event.Tags)
            {
                var tag = await _appContext.Tags.FirstOrDefaultAsync(t => string.Equals(t.Name, tagName, StringComparison.CurrentCultureIgnoreCase));
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

        public async Task UpdateEvent(EventUpdateDTO @event, string hostRoot)
        {
            var update = await _appContext.Events.FirstOrDefaultAsync(e => e.Id == @event.Id);
            if (update == null)
            {
                throw new NullReferenceException($"Event with id={@event.Id} not found");
            }
            _mapper.Map(@event, update);
            await _appContext.SaveChangesAsync(default);

            if (@event.Image != null)
            {
                string path = $"{hostRoot}\\wwroot\\img\\events\\{update.Id}.jpg";
                await using var fileStream = new FileStream(path, FileMode.Create);
                await @event.Image.CopyToAsync(fileStream);
            }

            _appContext.EventsTags.RemoveRange(_appContext.EventsTags.Where(et => et.EventId == @event.Id));
            foreach (var tagName in @event.Tags)
            {
                var tag = await _appContext.Tags.FirstOrDefaultAsync(t => string.Equals(t.Name, tagName, StringComparison.CurrentCultureIgnoreCase));
                if (tag == null)
                {
                    tag = _mapper.Map<string, Tag>(tagName);
                    await _appContext.Tags.AddAsync(tag);
                    await _appContext.SaveChangesAsync(default);
                }
                await _appContext.EventsTags.AddAsync(new EventsTags { EventId = update.Id, TagId = tag.Id });
            }
            await _appContext.SaveChangesAsync(default);
        }

        public async Task<EventToUpdateDTO> GetEventToUpdate(int eventId)
        {
            var @event = await _appContext.Events.Include(e=>e.Tags).FirstOrDefaultAsync(e => e.Id == eventId);
            if (@event == null)
            {
                throw new NullReferenceException($"Event with id={eventId} not found");
            }
            EventToUpdateDTO eventToUpdate = _mapper.Map<EventToUpdateDTO>(@event);

            if (@event.HasImage)
            {
                eventToUpdate.Image = $"img\\events\\{eventId}.jpg";
            }
            var tags = _appContext.EventsTags.Include(et => et.Tag).Where(et => et.TagId == eventId).Select(et => et.Tag.Name).ToHashSet();
            eventToUpdate.Tags = tags;
            return eventToUpdate;
        }

        public async Task DeleteEvent(int eventId, bool force, string hostRoot)
        {
            var @event = await _appContext.Events.IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Id == eventId);
            if (@event == null)
            {
                throw new NullReferenceException($"Event with id={eventId} not found");
            }
            if (force)
            {
                _appContext.Events.Remove(@event);
                string path = $"{hostRoot}\\wwroot\\img\\events\\{eventId}.jpg";
                File.Delete(path);
            }
            else
            {
                @event.IsDeleted = true;
            }
            await _appContext.SaveChangesAsync(default);
        }

        public async Task<EventFullDTO> GetEvent(int eventId)
        {
            var DBEvent = await _appContext.Events.Include(e => e.Organizer).Include(e => e.Category).FirstOrDefaultAsync(e => e.Id == eventId);
            if (DBEvent == null)
            {
                throw new NullReferenceException($"Event with id={eventId} not found");
            }
            var eventFullDTO = _mapper.Map<EventFullDTO>(DBEvent);

            var participants = await _appContext.EventsUsers.Include(eu => eu.Participant).Where(eu => eu.EventId == eventId).Select(eu => eu.Participant).ToListAsync();
            foreach (var participant in participants)
            {
                eventFullDTO.Participants.Add(participant.Id.ToString(), participant.UserName);
            }
            var tags = _appContext.EventsTags.Include(et => et.Tag).Where(et => et.EventId == eventId).Select(et => et.Tag.Name).ToHashSet();
            eventFullDTO.Tags = tags;

            if (DBEvent.HasImage)
            {
                eventFullDTO.Image = $"img\\events\\{eventId}.jpg";
            }

            return eventFullDTO;
        }

        public async Task<Page<EventLiteDTO>> GetEvents(int index, int pageSize, string search, int? categoryId, string tag, bool? upComing, bool onlyFree,
            bool vacancies, Guid? organizer, Guid? participant)
        {
            var result = new Page<EventLiteDTO>() { CurrentPage = index, PageSize = pageSize };           
            var query = _appContext.Events.Include(e => e.Category).AsQueryable();
            if (search != null)
            {
                query = query.Where(e => e.Name.ToLower().Contains(search.ToLower()));
            }
            if (categoryId != null)
            {
                query = query.Where(e => e.CategoryId == categoryId);
            }
            if (tag != null)
            {
                query = query.Where(e => _appContext.EventsTags.Include(e=>e.Tag).Where(et=>et.EventId==e.Id).Any(et => et.Tag.Name == tag));
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
                query = query.Where(e => e.ParticipantsLimit == 0 || _appContext.EventsUsers.Where(eu => eu.EventId == e.Id).Count() < e.ParticipantsLimit);
            }
            if (organizer != null)
            {
                query = query.Where(e => e.OrganizerId == organizer);
            }
            if (participant != null)
            {
                query = query.Where(e => e.Participants.Any(p => p.Id == participant));
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
            result.Records = await _mapper.ProjectTo<EventLiteDTO>(query).ToListAsync(default);
           
            for (int i = 0; i < result.TotalRecords; i++)
            {
                if (result.Records[i].HasImage)
                {
                    string path = $"img\\events\\{result.Records[i].Id}.jpg";
                    result.Records[i].Image = path;
                }
            }
            return result;
        }

        public async Task Subscribe(Guid userId, int eventId)
        {
            if (!await _appContext.Users.AnyAsync(u => Guid.Equals(u.Id, userId)))
            {
                throw new NullReferenceException($"User with id={userId} not found");
            }
            if (!await _appContext.Events.AnyAsync(e => e.Id == eventId))
            {
                throw new NullReferenceException($"Event with id={eventId} not found");
            }
            if (await _appContext.EventsUsers.AnyAsync(eu => eu.ParticipantId == userId && eu.EventId == eventId))
            {
                throw new ArgumentException($"User(id={userId}) is already signed up on event(id={eventId})");
            }
            if (await _appContext.EventsUsers.Where(eu => eu.EventId == eventId).CountAsync() >= (await _appContext.Events.FirstAsync(e => e.Id == eventId)).ParticipantsLimit)
            {
                throw new ArgumentException($"No vacancies on event(id={eventId})");
            }
            if ((await _appContext.Events.FirstAsync(e => e.Id == eventId)).Start < DateTime.Now)
            {
                throw new ArgumentOutOfRangeException($"Event(id={eventId}) has already started");
            }
            var eu = new EventsUsers { ParticipantId = userId, EventId = eventId};
            await _appContext.EventsUsers.AddAsync(eu);
            await _appContext.SaveChangesAsync(default);
        }

        public async Task Unsubscribe(Guid userId, int eventId)
        {
            if (!await _appContext.Users.AnyAsync(u => Guid.Equals(u.Id, userId)))
            {
                throw new NullReferenceException($"User with id={userId} not found");
            }
            if (!await _appContext.Events.AnyAsync(e => e.Id == eventId))
            {
                throw new NullReferenceException($"Event with id={eventId} not found");
            }
            if (!await _appContext.EventsUsers.AnyAsync(eu => eu.ParticipantId == userId && eu.EventId == eventId))
            {
                throw new NullReferenceException($"User(id={userId}) is not signed up on event(id={eventId})");
            }
            if ((await _appContext.Events.FirstAsync(e => e.Id == eventId)).Start < DateTime.Now)
            {
                throw new ArgumentOutOfRangeException($"Event(id={eventId}) has already started");
            }

            var eu = await _appContext.EventsUsers.FirstOrDefaultAsync(eu => eu.EventId == eventId && eu.ParticipantId == userId);
            if (eu != null)
            {
                _appContext.EventsUsers.Remove(eu);
            }
            await _appContext.SaveChangesAsync(default);
        }

        public async Task<Guid?> GetEventOrganizerId(int eventId)
        {
            if (!await _appContext.Events.AnyAsync(e => e.Id == eventId))
            {
                throw new NullReferenceException($"Event with id={eventId} not found");
            }
            return (await _appContext.Events.FirstOrDefaultAsync(e => e.Id == eventId))?.OrganizerId;
        }
    }
}
