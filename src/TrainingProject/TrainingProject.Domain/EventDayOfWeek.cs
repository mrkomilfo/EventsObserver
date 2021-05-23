using System;
using System.Collections.Generic;
using System.Linq;

using static TrainingProject.Domain.Helpers.NumberHelper;

namespace TrainingProject.Domain
{
    public class EventDayOfWeek
    {
        public int Id { get; set; }

        public int EventId { get; set; }

        public DayOfWeek DayOfWeek { get; set; }

        public TimeSpan Start { get; set; }

        public Event Event { get; set; }

        public ICollection<EventDayOfWeekParticipant> RecurrentEventParticipants { get; set; }

        public EventDayOfWeek()
        {
            RecurrentEventParticipants = new HashSet<EventDayOfWeekParticipant>();
        }

        public DateTime GetNearestDateTime()
        {
            int daysBefore;

            if (DayOfWeek == DateTime.Now.DayOfWeek)
            {
                daysBefore = Start > DateTime.Now.TimeOfDay ? 0 : 7;
            }
            else
            {
                daysBefore = TrueModulo(DayOfWeek - DateTime.Now.DayOfWeek, 7);
            }

            var dateTime = DateTime.Now.Date.AddDays(daysBefore) + Start;

            return dateTime;
        }

        public int GetParticipantsQuantity()
        {
            return RecurrentEventParticipants?.Count(x => x.RegistrationDateTime.AddDays(7) > GetNearestDateTime()) ?? 0;
        }
    }
}
