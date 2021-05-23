using System;

namespace TrainingProject.Domain
{
    public class EventDayOfWeekParticipant
    {
        public int Id { get; set; }

        public int EventDayOfWeekId { get; set; }

        public Guid ParticipantId { get; set; }

        public DateTime RegistrationDateTime { get; set; }

        public string Code { get; set; }

        public bool IsChecked { get; set; }

        public EventDayOfWeek EventDayOfWeek { get; set; }

        public User Participant { get; set; }
    }
}