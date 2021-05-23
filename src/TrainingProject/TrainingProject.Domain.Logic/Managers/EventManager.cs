using AutoMapper;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;

using TrainingProject.Common;
using TrainingProject.Data;
using TrainingProject.Domain;
using TrainingProject.DomainLogic.Helpers;
using TrainingProject.DomainLogic.Interfaces;
using TrainingProject.DomainLogic.Models.Common;
using TrainingProject.DomainLogic.Models.Events;
using TrainingProject.DomainLogic.Models.Users;
using TrainingProject.DomainLogic.Services;

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

        public async Task AddEventAsync(EventCreateDto eventDto, string hostRoot)
        {
            _logger.LogMethodCallingWithObject(eventDto);

            if (!eventDto.IsRecurrent && string.IsNullOrEmpty(eventDto.Start))
            {
                throw new ArgumentNullException(nameof(eventDto.Start));
            }

            var newEvent = _mapper.Map<EventCreateDto, Event>(eventDto);

            await _appContext.Events.AddAsync(newEvent);
            await _appContext.SaveChangesAsync(default);

            if (eventDto.Image != null)
            {
                var path = $"{hostRoot}\\wwwroot\\img\\events\\{newEvent.Id}.jpg";

                _logger.LogInfo($"Saving image to {path}");

                await using var fileStream = new FileStream(path, FileMode.Create);

                await eventDto.Image.CopyToAsync(fileStream);
            }

            if (eventDto.Tags != null)
            {
                var tags = eventDto.Tags.ParseSubstrings(",");

                foreach (var tagName in tags)
                {
                    var tag = await _appContext.Tags.FirstOrDefaultAsync(t => string.Equals(t.Name.ToLower(), tagName.ToLower()));

                    if (tag == null)
                    {
                        tag = _mapper.Map<string, Tag>(tagName);
                        await _appContext.Tags.AddAsync(tag);
                        await _appContext.SaveChangesAsync(default);
                    }

                    await _appContext.EventsTags.AddAsync(new EventTag { EventId = newEvent.Id, TagId = tag.Id });
                }
            }

            if (eventDto.IsRecurrent)
            {
                if (string.IsNullOrEmpty(eventDto.EventDaysOfWeek))
                {
                    throw new ArgumentNullException(nameof(eventDto.EventDaysOfWeek));
                }
                
                var deserializedEventDaysOfWeek = 
                    JsonConvert.DeserializeObject<List<EventDayOfWeekPostDto>>(eventDto.EventDaysOfWeek);
                var eventDaysOfWeek = deserializedEventDaysOfWeek.Select(x => new EventDayOfWeek
                {
                    EventId = newEvent.Id,
                    DayOfWeek = (DayOfWeek) x.DayOfWeek,
                    Start = TimeSpan.Parse(x.Start)
                });

                await _appContext.EventDaysOfWeek.AddRangeAsync(eventDaysOfWeek);
            }

            await _appContext.SaveChangesAsync(default);
        }

        public async Task UpdateEventAsync(EventUpdateDto eventDto, string hostRoot)
        {
            _logger.LogMethodCallingWithObject(eventDto);

            var eventToUpdate = await _appContext.Events.FirstOrDefaultAsync(e => e.Id == eventDto.Id);

            if (eventToUpdate == null)
            {
                throw new KeyNotFoundException($"Event with id={eventDto.Id} not found");
            }

            var subscribesCount = await _appContext.EventsParticipants.Where(eu => eu.EventId == eventDto.Id).CountAsync();

            if (eventDto.ParticipantsLimit < subscribesCount && eventDto.ParticipantsLimit != 0)
            {
                throw new ArgumentOutOfRangeException($"Сurrent number of participants({subscribesCount}) is greater than the new limit({eventDto.ParticipantsLimit})");
            }

            _mapper.Map(eventDto, eventToUpdate);

            await _appContext.SaveChangesAsync(default);

            _appContext.EventsTags.RemoveRange(_appContext.EventsTags.Where(et => et.EventId == eventDto.Id));

            if (eventDto.Tags != null)
            {
                var tags = eventDto.Tags.ParseSubstrings(",");

                foreach (var tagName in tags)
                {
                    var tag = await _appContext.Tags.FirstOrDefaultAsync(t => string.Equals(t.Name.ToLower(), tagName));

                    if (tag == null)
                    {
                        tag = _mapper.Map<string, Tag>(tagName);

                        await _appContext.Tags.AddAsync(tag);
                        await _appContext.SaveChangesAsync(default);
                    }

                    await _appContext.EventsTags.AddAsync(new EventTag { EventId = eventToUpdate.Id, TagId = tag.Id });
                }
            }

            if (eventDto.Image != null)
            {
                var path = $"{hostRoot}\\wwwroot\\img\\events\\{eventToUpdate.Id}.jpg";

                _logger.LogInfo($"Saving image to {path}");

                await using var fileStream = new FileStream(path, FileMode.Create);

                await eventDto.Image.CopyToAsync(fileStream);
            }

            var existingEventDaysOfWeek = _appContext.EventDaysOfWeek.Where(x => x.EventId == eventDto.Id);

            if (existingEventDaysOfWeek.Any())
            {
                _appContext.EventDaysOfWeek.RemoveRange(existingEventDaysOfWeek);
        
                await _appContext.SaveChangesAsync(default);
            }

            if (eventDto.IsRecurrent)
            {
                if (string.IsNullOrEmpty(eventDto.EventDaysOfWeek))
                {
                    throw new ArgumentNullException(nameof(eventDto.EventDaysOfWeek));
                }
                
                var deserializedEventDaysOfWeek = 
                    JsonConvert.DeserializeObject<List<EventDayOfWeekPostDto>>(eventDto.EventDaysOfWeek);
                var eventDaysOfWeek = deserializedEventDaysOfWeek.Select(x => new EventDayOfWeek
                {
                    EventId = eventDto.Id,
                    DayOfWeek = (DayOfWeek) x.DayOfWeek,
                    Start = TimeSpan.Parse(x.Start)
                });

                await _appContext.EventDaysOfWeek.AddRangeAsync(eventDaysOfWeek);
            }

            await _appContext.SaveChangesAsync(default);
        }

        public async Task<EventToUpdateDto> GetEventToUpdateAsync(int eventId)
        {
            _logger.LogMethodCallingWithObject(new { eventId });

            var @event = await _appContext.Events.Include(x => x.DaysOfWeek)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (@event == null)
            {
                throw new KeyNotFoundException($"Event with id={eventId} not found");
            }

            var eventToUpdate = _mapper.Map<EventToUpdateDto>(@event);

            if (@event.HasImage)
            {
                eventToUpdate.Image = $"img\\events\\{eventId}.jpg";
            }

            var tags = _appContext.EventsTags
                .Include(et => et.Tag)
                .Where(et => et.EventId == eventId)
                .Select(et => et.Tag.Name).ToHashSet();
            
            eventToUpdate.Tags = string.Join(", ", tags);

            if (@event.IsRecurrent)
            {
                eventToUpdate.WeekDays =
                    JsonConvert.SerializeObject(GetWeekDaysByTime(@event.DaysOfWeek), Formatting.Indented);
            }

            return eventToUpdate;
        }

        public async Task DeleteEventAsync(int eventId, bool force, string hostRoot)
        {
            _logger.LogMethodCallingWithObject(new { eventId, force, hostRoot });

            var @event = await _appContext.Events.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Id == eventId);

            if (@event == null)
            {
                throw new KeyNotFoundException($"Event with id={eventId} not found");
            }

            if (force)
            {
                _appContext.Events.Remove(@event);
        
                var path = $"{hostRoot}\\wwwroot\\img\\events\\{eventId}.jpg";
        
                _logger.LogInfo($"Deleting image from {path}");
        
                File.Delete(path);
            }
            else
            {
                @event.IsDeleted = true;
            }

            await _appContext.SaveChangesAsync(default);
        }

        public async Task<EventFullDto> GetEventAsync(int eventId, string userId = null)
        {
            _logger.LogMethodCallingWithObject(new { eventId });

            var dbEvent = await _appContext.Events
                .Include(e => e.Organizer)
                .Include(e => e.Category)
                .Include(e => e.DaysOfWeek)
                    .ThenInclude(e => e.RecurrentEventParticipants)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (dbEvent == null)
            {
                throw new KeyNotFoundException($"Event with id={eventId} not found");
            }

            var eventFullDto = _mapper.Map<EventFullDto>(dbEvent);

            var tags = _appContext.EventsTags.Include(et => et.Tag).Where(et => et.EventId == eventId).Select(et => et.Tag).ToHashSet();

            foreach (var tag in tags)
            {
                eventFullDto.Tags.Add(tag.Id.ToString(), tag.Name);
            }

            if (dbEvent.HasImage)
            {
                eventFullDto.Image = $"img\\events\\{eventId}.jpg";
            }

            if (dbEvent.IsRecurrent)
            {
                eventFullDto.WeekDays = _mapper.Map<IReadOnlyCollection<EventDayOfWeekDto>>(dbEvent.DaysOfWeek);

                if (!string.IsNullOrEmpty(userId))
                {
                    foreach (var weekDay in eventFullDto.WeekDays)
                    {
                        weekDay.AmISubscribed = await IsUserSubscribedOnRecurrentEvent(weekDay.Id, userId);
                    }
                }
            }
            else
            {
                eventFullDto.Participants = _appContext.EventsParticipants.Include(eu => eu.Participant)
                    .Where(eu => eu.EventId == eventId).Select(eu => eu.Participant).Count();

                if (!string.IsNullOrEmpty(userId))
                {
                    eventFullDto.AmISubscribed = await IsUserSubscribedOnRecurrentEvent(eventFullDto.Id, userId);
                }
            }

            return eventFullDto;
        }

        public async Task<Page<EventLiteDto>> GetEventsAsync(int index, int pageSize, string search, int? categoryId, string tag,
            bool? upComing, bool onlyFree, bool vacancies, string organizerId, string participantId)
        {
            _logger.LogMethodCallingWithObject(new { index, pageSize, search, categoryId, tag, upComing, onlyFree, vacancies, organizerId, participantId });

            var organizerGuid = string.IsNullOrEmpty(organizerId) ? new Guid() : Guid.Parse(organizerId);
            var participantGuid = string.IsNullOrEmpty(participantId) ? new Guid() : Guid.Parse(participantId);
            var result = new Page<EventLiteDto> { CurrentPage = index, PageSize = pageSize };
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
                query = query.Where(e => _appContext.EventsTags.Include(et => et.Tag)
                    .Where(et => et.EventId == e.Id)
                    .Any(et => Equals(et.Tag.Name, tag)));
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
                query = query.Where(e => e.ParticipantsLimit == 0 || _appContext.EventsParticipants.Count(eu => eu.EventId == e.Id) < e.ParticipantsLimit);
            }

            if (organizerGuid != Guid.Empty)
            {
                query = query.Where(e => Equals(e.OrganizerId, organizerGuid));
            }

            if (participantGuid != Guid.Empty)
            {
                query = query.Where(e => _appContext.EventsParticipants.Include(eu => eu.Participant)
                    .Where(eu => eu.EventId == e.Id)
                    .Any(eu => Equals(eu.ParticipantId, participantGuid)));
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

            result.Records = await _mapper.ProjectTo<EventLiteDto>(query).ToListAsync();

            foreach (var t in result.Records)
            {
                if (t.HasImage)
                {
                    var path = $"img\\events\\{t.Id}.jpg";

                    t.Image = path;
                }
            }

            return result;
        }

        public async Task SubscribeAsync(Guid userId, int eventId)
        {
            _logger.LogMethodCallingWithObject(new { userId, eventId });

            if (!await _appContext.Users.AnyAsync(u => Equals(u.Id, userId)))
            {
                throw new KeyNotFoundException($"User with id={userId} not found");
            }

            if (!await _appContext.Events.AnyAsync(e => e.Id == eventId && e.IsRecurrent == false))
            {
                throw new KeyNotFoundException($"Event with id={eventId} not found");
            }

            if (await _appContext.EventsParticipants.AnyAsync(eu => Equals(eu.ParticipantId, userId) && eu.EventId == eventId))
            {
                throw new ArgumentOutOfRangeException($"User(id={userId}) is already signed up on event(id={eventId})");
            }

            var participantsLimit = (await _appContext.Events.FirstAsync(e => e.Id == eventId)).ParticipantsLimit;

            if (await _appContext.EventsParticipants.CountAsync(eu => eu.EventId == eventId) >= participantsLimit && participantsLimit != 0)
            {
                throw new ArgumentOutOfRangeException($"No vacancies on event(id={eventId})");
            }

            if ((await _appContext.Events.FirstAsync(e => e.Id == eventId)).Start < DateTime.Now)
            {
                throw new AccessViolationException($"Event(id={eventId}) has already started");
            }

            var eu = new EventParticipant
            {
                ParticipantId = userId, 
                EventId = eventId, 
                IsChecked = false, 
                Code = CodeGenerator.GenerateCode(4)
            };

            await _appContext.EventsParticipants.AddAsync(eu); 
            await _appContext.SaveChangesAsync(default);
        }

        public async Task UnsubscribeAsync(Guid userId, int eventId)
        {
            _logger.LogMethodCallingWithObject(new { userId, eventId });

            if (!await _appContext.EventsParticipants.AnyAsync(eu => Equals(eu.ParticipantId, userId) && eu.EventId == eventId))
            {
                throw new KeyNotFoundException($"User(id={userId}) is not signed up on event(id={eventId})");
            }

            if ((await _appContext.Events.FirstAsync(e => e.Id == eventId)).Start < DateTime.Now)
            {
                throw new AccessViolationException($"Event(id={eventId}) has already started");
            }

            var eu = await _appContext.EventsParticipants
                .FirstOrDefaultAsync(eu => eu.EventId == eventId && Equals(eu.ParticipantId, userId));

            if (eu != null)
            {
                _appContext.EventsParticipants.Remove(eu);
            }

            await _appContext.SaveChangesAsync(default);
        }

        public async Task<Guid?> GetEventOrganizerIdAsync(int eventId)
        {
            _logger.LogMethodCallingWithObject(new { eventId });

            if (!await _appContext.Events.AnyAsync(e => e.Id == eventId))
            {
                throw new KeyNotFoundException($"Event with id={eventId} not found");
            }

            return (await _appContext.Events.FirstOrDefaultAsync(e => e.Id == eventId))?.OrganizerId;
        }

        public void Notify()
        {
            _logger.LogMethodCalling();

            var now = DateTime.Now;
            var eventUsers = _appContext.EventsParticipants.Include(eu => eu.Event).Include(eu => eu.Participant).ToList();
            var eventUsersList = eventUsers
                .Where(eu => !string.IsNullOrEmpty(eu.Participant.Email)
                    && eu.Event.Start > now.AddHours(23)
                    && eu.Event.Start <= now.AddHours(24));

            foreach (var eu in eventUsersList)
            {
                _notificator.NotifyAboutEvent(eu);
            }
        }

        public async Task SubscribeOnRecurrentEventAsync(Guid userId, int eventDayOfWeekId)
        {
            _logger.LogMethodCallingWithObject(new { userId, eventDayOfWeekId });

            if (!await _appContext.Users.AnyAsync(u => Equals(u.Id, userId)))
            {
                throw new KeyNotFoundException(nameof(userId));
            }

            if (!await _appContext.EventDaysOfWeek.AnyAsync(e => e.Id == eventDayOfWeekId))
            {
                throw new KeyNotFoundException(nameof(eventDayOfWeekId));
            }

            var eventDayOfWeekParticipant = new EventDayOfWeekParticipant
            {
                EventDayOfWeekId = eventDayOfWeekId,
                ParticipantId = userId,
                RegistrationDateTime = DateTime.Now,
                IsChecked = false,
                Code = CodeGenerator.GenerateCode(4)
            };

            await _appContext.RecurrentEventParticipants.AddAsync(eventDayOfWeekParticipant);
            await _appContext.SaveChangesAsync(default);
        }

        public async Task UnsubscribeFromRecurrentEventAsync(Guid userId, int eventDayOfWeekId)
        {
            _logger.LogMethodCallingWithObject(new { userId, eventDayOfWeekId });

            if (!await _appContext.Users.AnyAsync(u => Equals(u.Id, userId)))
            {
                throw new KeyNotFoundException(nameof(userId));
            }

            if (!await _appContext.EventDaysOfWeek.AnyAsync(e => e.Id == eventDayOfWeekId))
            {
                throw new KeyNotFoundException(nameof(eventDayOfWeekId));
            }

            var eventDayOfWeek = await _appContext.EventDaysOfWeek.FindAsync(eventDayOfWeekId);

            if (eventDayOfWeek == null)
            {
                throw new KeyNotFoundException(nameof(eventDayOfWeekId));
            }

            var recurrentEventParticipant = await _appContext.RecurrentEventParticipants.FirstOrDefaultAsync(x =>
                x.EventDayOfWeekId == eventDayOfWeekId &&
                x.ParticipantId == userId &&
                x.RegistrationDateTime.AddDays(7) > eventDayOfWeek.GetNearestDateTime());

            if (recurrentEventParticipant == null)
            {
                throw new ObjectNotFoundException(nameof(recurrentEventParticipant));
            }

            _appContext.RecurrentEventParticipants.Remove(recurrentEventParticipant);
            await _appContext.SaveChangesAsync(default);
        }

        public async Task<IEnumerable<ParticipantDto>> GetEventParticipants(int eventId)
        {
            _logger.LogMethodCallingWithObject(new {eventId});

            var eventDayOfWeek = await _appContext.Events.FindAsync(eventId);

            if (eventDayOfWeek == null)
            {
                throw new KeyNotFoundException(nameof(eventId));
            }

            var participants = await _appContext.EventsParticipants
                .Include(eu => eu.Participant)
                .Where(eu => eu.EventId == eventId).ToListAsync();

            return _mapper.Map<IEnumerable<ParticipantDto>>(participants);
        }

        public async Task<IEnumerable<ParticipantDto>> GetRecurrentEventParticipants(int eventDayOfWeekId)
        {
            _logger.LogMethodCallingWithObject(new {eventDayOfWeekId});

            var eventDayOfWeek = await _appContext.EventDaysOfWeek.FindAsync(eventDayOfWeekId);

            if (eventDayOfWeek == null)
            {
                throw new KeyNotFoundException(nameof(eventDayOfWeekId));
            }

            var recurrentEventParticipants = await _appContext.RecurrentEventParticipants
                .Include(x => x.Participant)
                .Where(x => x.RegistrationDateTime.AddDays(7) > eventDayOfWeek.GetNearestDateTime()).ToListAsync();

            return _mapper.Map<IEnumerable<ParticipantDto>>(recurrentEventParticipants);
        }

        public async Task ToggleEventApprove(int eventId)
        {
            _logger.LogMethodCallingWithObject(new { eventId });

            var dbEvent = await _appContext.Events.FindAsync(eventId);

            if (dbEvent == null)
            {
                throw new KeyNotFoundException($"Event with id={eventId} not found");
            }

            dbEvent.IsApproved = !dbEvent.IsApproved;

            await _appContext.SaveChangesAsync(default);
        }

        public async Task ToggleParticipantCheck(int id)
        {
            _logger.LogMethodCallingWithObject(new { id });

            var eventParticipant = await _appContext.EventsParticipants.FindAsync(id);

            if (eventParticipant == null)
            {
                throw new KeyNotFoundException(nameof(eventParticipant));
            }

            eventParticipant.IsChecked = !eventParticipant.IsChecked;

            await _appContext.SaveChangesAsync(default);
        }

        public async Task ToggleRecurrentEventParticipantCheck(int id)
        {
            _logger.LogMethodCallingWithObject(new { id });

            var eventParticipant = await _appContext.RecurrentEventParticipants.FindAsync(id);

            if (eventParticipant == null)
            {
                throw new KeyNotFoundException(nameof(eventParticipant));
            }

            eventParticipant.IsChecked = !eventParticipant.IsChecked;

            await _appContext.SaveChangesAsync(default);
        }

        private async Task<bool> IsUserSubscribedOnEvent(int eventId, string userId)
        {
            return (await GetEventParticipants(eventId)).Any(p => p.UserId == userId);
        }

        private async Task<bool> IsUserSubscribedOnRecurrentEvent(int eventDayOfWeekId, string userId)
        {
            return (await GetRecurrentEventParticipants(eventDayOfWeekId)).Any(p => p.UserId == userId);
        }

        private static Dictionary<int, string> GetWeekDaysByTime(IEnumerable<EventDayOfWeek> weekDays)
        {
            return weekDays == null
                ? new Dictionary<int, string>()
                : weekDays.ToDictionary(x => (int) x.DayOfWeek, x => x.Start.ToString());
        }


    }
}
