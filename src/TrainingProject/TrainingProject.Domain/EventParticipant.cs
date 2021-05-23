using System;

namespace TrainingProject.Domain
{
    public class EventParticipant
    {
        public int Id { get; set; }

        public int EventId { get; set; }

        public Guid ParticipantId { get; set; }

        public string Code { get; set; }

        public bool IsChecked { get; set; }

        public Event Event { get; set; }

        public User Participant { get; set; }
    }
}
