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
using TrainingProject.DomainLogic.Helpers;
using TrainingProject.DomainLogic.Services;
using System.Collections.Generic;
using TrainingProject.Common;

namespace TrainingProject.DomainLogic.Managers
{
    public class EventManager : IEventManager
    {
        private readonly IAppContext _appContext;
        private readonly IMapper _mapper;
        private readonly INotificator _notificator;
        private readonly ILogHelper _logger;

        public EventManager(IAppContext appContext, IMapper mapper, INotificator notificator, ILogHelper logger)
        {
            _appContext = appContext;
            _mapper = mapper;
            _notificator = notificator;
            _logger = logger;
        }

        public async Task AddEvent(EventCreateDTO @event, string hostRoot)
        {
            _logger.LogMethodCallingWithObject(@event);
            var newEvent = _mapper.Map<EventCreateDTO, Event>(@event);
            await _appContext.Events.AddAsync(newEvent);
            await _appContext.SaveChangesAsync(default);

            if (@event.Image != null)
            {
                string path = $"{hostRoot}\\wwwroot\\img\\events\\{newEvent.Id}.jpg";
                _logger.LogInfo($"Saving image to {path}");
                await using var fileStream = new FileStream(path, FileMode.Create);
                await @event.Image.CopyToAsync(fileStream);
            }

            if (@event.Tags != null)
            {
                var tags = @event.Tags.ParseSubstrings(",");
                foreach (var tagName in tags)
                {
                    var tag = await _appContext.Tags.FirstOrDefaultAsync(t => string.Equals(t.Name.ToLower(), tagName.ToLower()));
                    if (tag == null)
                    {
                        tag = _mapper.Map<string, Tag>(tagName);
                        await _appContext.Tags.AddAsync(tag);
                        await _appContext.SaveChangesAsync(default);
                    }
                    await _appContext.EventsTags.AddAsync(new EventsTags { EventId = newEvent.Id, TagId = tag.Id });
                }
            }

            await _appContext.SaveChangesAsync(default);
        }

        public async Task UpdateEvent(EventUpdateDTO @event, string hostRoot)
        {
            _logger.LogMethodCallingWithObject(@event);
            var update = await _appContext.Events.FirstOrDefaultAsync(e => e.Id == @event.Id);
            if (update == null)
            {
                throw new KeyNotFoundException($"Event with id={@event.Id} not found");
            }
            int subscribesCount = await _appContext.EventsUsers.Where(eu => eu.EventId == @event.Id).CountAsync();
            if (@event.ParticipantsLimit < subscribesCount && @event.ParticipantsLimit != 0)
            {
                throw new ArgumentException($"Сurrent number of participants({subscribesCount}) is greater than the new limit({@event.ParticipantsLimit})");
            }
            _mapper.Map(@event, update);
            await _appContext.SaveChangesAsync(default);

            _appContext.EventsTags.RemoveRange(_appContext.EventsTags.Where(et => et.EventId == @event.Id));
            if (@event.Tags != null)
            {
                var tags = @event.Tags.ParseSubstrings(",");
                foreach (var tagName in tags)
                {
                    var tag = await _appContext.Tags.FirstOrDefaultAsync(t => string.Equals(t.Name.ToLower(), tagName));
                    if (tag == null)
                    {
                        tag = _mapper.Map<string, Tag>(tagName);
                        await _appContext.Tags.AddAsync(tag);
                        await _appContext.SaveChangesAsync(default);
                    }
                    await _appContext.EventsTags.AddAsync(new EventsTags { EventId = update.Id, TagId = tag.Id });
                }
            }

            if (@event.Image != null)
            {
                string path = $"{hostRoot}\\wwwroot\\img\\events\\{update.Id}.jpg";
                _logger.LogInfo($"Saving image to {path}");
                await using var fileStream = new FileStream(path, FileMode.Create);
                await @event.Image.CopyToAsync(fileStream);
            }

            await _appContext.SaveChangesAsync(default);
        }

        public async Task<EventToUpdateDTO> GetEventToUpdate(int eventId)
        {
            _logger.LogMethodCallingWithObject(new { eventId });
            var @event = await _appContext.Events.FirstOrDefaultAsync(e => e.Id == eventId);
            if (@event == null)
            {
                throw new KeyNotFoundException($"Event with id={eventId} not found");
            }
            EventToUpdateDTO eventToUpdate = _mapper.Map<EventToUpdateDTO>(@event);

            if (@event.HasImage)
            {
                eventToUpdate.Image = $"img\\events\\{eventId}.jpg";
            }
            var tags = _appContext.EventsTags.Include(et => et.Tag).Where(et => et.EventId == eventId).Select(et => et.Tag.Name).ToHashSet();
            eventToUpdate.Tags = String.Join(", ", tags);
            return eventToUpdate;
        }

        public async Task DeleteEvent(int eventId, bool force, string hostRoot)
        {
            _logger.LogMethodCallingWithObject(new { eventId, force, hostRoot });
            var @event = await _appContext.Events.IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Id == eventId);
            if (@event == null)
            {
                throw new KeyNotFoundException($"Event with id={eventId} not found");
            }
            if (force)
            {
                _appContext.Events.Remove(@event);
                string path = $"{hostRoot}\\wwwroot\\img\\events\\{eventId}.jpg";
                _logger.LogInfo($"Deleting image from {path}");
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
            _logger.LogMethodCallingWithObject(new { eventId });
            var DBEvent = await _appContext.Events.Include(e => e.Organizer).Include(e => e.Category).FirstOrDefaultAsync(e => e.Id == eventId);
            if (DBEvent == null)
            {
                throw new KeyNotFoundException($"Event with id={eventId} not found");
            }
            var eventFullDTO = _mapper.Map<EventFullDTO>(DBEvent);

            var participants = await _appContext.EventsUsers.Include(eu => eu.Participant).Where(eu => eu.EventId == eventId).Select(eu => eu.Participant).ToListAsync();
            foreach (var participant in participants)
            {
                eventFullDTO.Participants.Add(participant.Id.ToString(), participant.UserName);
            }
            var tags = _appContext.EventsTags.Include(et => et.Tag).Where(et => et.EventId == eventId).Select(et => et.Tag).ToHashSet();
            foreach (var tag in tags)
            {
                eventFullDTO.Tags.Add(tag.Id.ToString(), tag.Name);
            }

            if (DBEvent.HasImage)
            {
                eventFullDTO.Image = $"img\\events\\{eventId}.jpg";
            }

            return eventFullDTO;
        }

        public async Task<Page<EventLiteDTO>> GetEvents(int index, int pageSize, string search, int? categoryId, string tag, 
            bool? upComing, bool onlyFree, bool vacancies, Guid organizerId = new Guid(), Guid participantId = new Guid())
        {
            _logger.LogMethodCallingWithObject(new { index, pageSize, search, categoryId, tag, upComing, onlyFree, vacancies, organizerId, participantId });
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
                query = query.Where(e => _appContext.EventsTags.Include(et => et.Tag).Where(et => String.Equals(et.EventId, e.Id)).Any(et => String.Equals(et.Tag.Name, tag)));
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
                query = query.Where(e => e.ParticipantsLimit == 0 || _appContext.EventsUsers.Count(eu => eu.EventId == e.Id) < e.ParticipantsLimit);
            }
            if (organizerId != Guid.Empty)
            {
                query = query.Where(e => Equals(e.OrganizerId, organizerId));
            }
            if (participantId != Guid.Empty)
            {
                query = query.Where(e => _appContext.EventsUsers.Include(eu => eu.Participant).Where(eu => Equals(eu.EventId, e.Id)).Any(eu => String.Equals(eu.ParticipantId, participantId)));
            }
            result.TotalRecords = await query.CountAsync();
            if (upComing != null && (bool)upComing)
            {
                query = query.OrderBy(e => e.Start).Skip(index * pageSize).Take(pageSize);
            }
            else
            {
                query = query.OrderByDescending(e => e.Start).Skip(index * pageSize).Take(pageSize);
            }
            result.Records = await _mapper.ProjectTo<EventLiteDTO>(query).ToListAsync(default);
           
            for (int i = 0; i < result.Records.Count; i++)
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
            _logger.LogMethodCallingWithObject(new { userId, eventId });
            if (!await _appContext.Users.AnyAsync(u => Equals(u.Id, userId)))
            {
                throw new KeyNotFoundException($"User with id={userId} not found");
            }
            if (!await _appContext.Events.AnyAsync(e => e.Id == eventId))
            {
                throw new KeyNotFoundException($"Event with id={eventId} not found");
            }
            if (await _appContext.EventsUsers.AnyAsync(eu => eu.ParticipantId == userId && eu.EventId == eventId))
            {
                throw new ArgumentException($"User(id={userId}) is already signed up on event(id={eventId})");
            }
            int participantsLimit = (await _appContext.Events.FirstAsync(e => e.Id == eventId)).ParticipantsLimit;
            if (await _appContext.EventsUsers.CountAsync(eu => eu.EventId == eventId) >= participantsLimit && participantsLimit != 0)
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
            _logger.LogMethodCallingWithObject(new { userId, eventId });
            if (!await _appContext.Users.AnyAsync(u => Equals(u.Id, userId)))
            {
                throw new KeyNotFoundException($"User with id={userId} not found");
            }
            if (!await _appContext.Events.AnyAsync(e => e.Id == eventId))
            {
                throw new KeyNotFoundException($"Event with id={eventId} not found");
            }
            if (!await _appContext.EventsUsers.AnyAsync(eu => Equals(eu.ParticipantId, userId) && eu.EventId == eventId))
            {
                throw new KeyNotFoundException($"User(id={userId}) is not signed up on event(id={eventId})");
            }
            if ((await _appContext.Events.FirstAsync(e => e.Id == eventId)).Start < DateTime.Now)
            {
                throw new ArgumentOutOfRangeException($"Event(id={eventId}) has already started");
            }

            var eu = await _appContext.EventsUsers.FirstOrDefaultAsync(eu => eu.EventId == eventId && Equals(eu.ParticipantId, userId));
            if (eu != null)
            {
                _appContext.EventsUsers.Remove(eu);
            }
            await _appContext.SaveChangesAsync(default);
        }

        public async Task<Guid?> GetEventOrganizerId(int eventId)
        {
            _logger.LogMethodCallingWithObject(new { eventId });
            if (!await _appContext.Events.AnyAsync(e => e.Id == eventId))
            {
                throw new KeyNotFoundException($"Event with id={eventId} not found");
            }
            return (await _appContext.Events.FirstOrDefaultAsync(e => e.Id == eventId))?.OrganizerId;
        }

        public void Notificate()
        {
            _logger.LogMethodCalling();
            DateTime now = DateTime.Now;
            var eventUsers = _appContext.EventsUsers.Include(eu => eu.Event).Include(eu => eu.Participant).ToList();
            var eventUsersList = eventUsers
                .Where(eu => !string.IsNullOrEmpty(eu.Participant.ContactEmail)
                    && eu.Event.Start > now.AddHours(23)
                    && eu.Event.Start <= now.AddHours(24));

            foreach (var eu in eventUsersList)
            {
                _notificator.Notificate(eu);
            }
        }

        public async Task<IList<string>> GetEventInvolvedUsersId(int eventId)
        {
            IList<string> ids = new List<string>();
            var targetEvent = await GetEvent(eventId);
            ids.Add(targetEvent.OrganizerId);
            var participantsIds = targetEvent.Participants.Keys;
            foreach (string participantId in participantsIds)
            {
                ids.Add(participantId);
            }
            return ids;
        }
    }
}
