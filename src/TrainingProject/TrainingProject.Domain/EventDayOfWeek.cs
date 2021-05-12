using System;
using System.Collections.Generic;

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
    }
}
