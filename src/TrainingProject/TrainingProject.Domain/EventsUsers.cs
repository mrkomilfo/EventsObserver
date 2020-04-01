using System;

namespace TrainingProject.Domain
{
    public class EventsUsers
    {
        public Event Event { get; set; }
        public int EventId { get; set; }
        public User Participant { get; set; }
        public Guid? ParticipantId { get; set; }
    }
}
