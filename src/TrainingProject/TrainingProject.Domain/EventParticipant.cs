using System;

namespace TrainingProject.Domain
{
    public class EventParticipant
    {
        public int EventId { get; set; }

        public Guid ParticipantId { get; set; }

        public Event Event { get; set; }

        public User Participant { get; set; }
    }
}
