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
using CSharpFunctionalExtensions;

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
                string path = $"{hostRoot}/img/events/{newEvent.Id}.jpg";
                await using var fileStream = new FileStream(path, FileMode.Create);
                await @event.Image.CopyToAsync(fileStream);
            }

            foreach (var tagName in @event.Tags)
            {
                var tag = await _appContext.Tags.FirstOrDefaultAsync(t => t.Name.ToLower() == tagName);
                if (tag == null)
                {
                    tag = _mapper.Map<string, Tag>(tagName.ToLower());
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
            if (update != null)
            {
                _mapper.Map(@event, update);
            }
            await _appContext.SaveChangesAsync(default);

            if (@event.Image != null)
            {
                string path = $"{hostRoot}/img/events/{update.Id}.jpg";
                await using var fileStream = new FileStream(path, FileMode.Create);
                await @event.Image.CopyToAsync(fileStream);
            }

            _appContext.EventsTags.RemoveRange(_appContext.EventsTags.Where(et => et.EventId == @event.Id));
            foreach (var tagName in @event.Tags)
            {
                var tag = await _appContext.Tags.FirstOrDefaultAsync(t => t.Name.ToLower() == tagName);
                if (tag == null)
                {
                    tag = _mapper.Map<string, Tag>(tagName.ToLower());
                    await _appContext.Tags.AddAsync(tag);
                    await _appContext.SaveChangesAsync(default);
                }
                await _appContext.EventsTags.AddAsync(new EventsTags { EventId = update.Id, TagId = tag.Id });
            }
            await _appContext.SaveChangesAsync(default);
        }

        public async Task DeleteEvent(int eventId, bool force, string hostRoot)
        {
            var @event = await _appContext.Events.IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Id == eventId);
            if (@event != null)
            {
                if (force)
                {
                    _appContext.Events.Remove(@event);
                    string path = $"{hostRoot}/img/events/{eventId}.jpg";
                    File.Delete(path);
                }
                else
                {
                    @event.IsDeleted = true;
                }
                await _appContext.SaveChangesAsync(default);
            }
        }

        public async Task<Maybe<EventFullDTO>> GetEvent(int eventId, string hostRoot)
        {
            var DBEvent = await _appContext.Events.Include(e=>e.Organizer).Include(e=>e.Category).FirstOrDefaultAsync(e => e.Id == eventId);
            if (DBEvent == null)
            {
                return null;
            }
            var eventFullDTO = _mapper.Map<EventFullDTO>(DBEvent);
            var participants = await _appContext.EventsUsers.Include(eu => eu.Participant).Where(eu => eu.EventId == eventId).Select(eu => eu.Participant).ToListAsync();
            foreach (var participant in participants)
            {
                eventFullDTO.Participants.Add(participant.Id, participant.UserName);
            }
            var tags = _appContext.EventsTags.Include(et => et.Tag).Where(et => et.TagId == eventId).Select(et => et.Tag);
            eventFullDTO.Tags = await _mapper.ProjectTo<string>(tags).ToListAsync(default);

            string imageName = DBEvent.HasPhoto ? eventId.ToString() : "default";
            string path = $"{hostRoot}/img/events/{imageName}.jpg";
            eventFullDTO.Photo = path;
            return eventFullDTO;
        }

        public async Task<Page<EventLiteDTO>> GetEvents(int index, int pageSize, string hostRoot, string search, byte? categoryId, string tag, bool? upComing, bool onlyFree,
            bool vacancies, int? organizer, int? participant)
        {
            var result = new Page<EventLiteDTO>() { CurrentPage = index, PageSize = pageSize };           
            var query = _appContext.Events.Include(e => e.Category).AsQueryable();
            if (search != null)
            {
                query = query.Where(e => e.Name.Contains(search, StringComparison.CurrentCultureIgnoreCase));
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
                query = query.Where(e => e.ParticipantsLimit == 0 || e.Participants.Count() < e.ParticipantsLimit);
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
                var tags = _appContext.EventsTags.Include(et => et.Tag).Where(et => et.TagId == result.Records[i].Id).Select(et => et.Tag);
                result.Records[i].Tags = await _mapper.ProjectTo<string>(tags).ToListAsync(default);

                string imageName = result.Records[i].HasPhoto ? result.Records[i].Id.ToString() : "default";
                string path = $"{hostRoot}/img/events/{imageName}.jpg";
                result.Records[i].Photo = path;
            }
            return result;
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
