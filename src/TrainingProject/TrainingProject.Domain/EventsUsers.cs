namespace TrainingProject.Domain
{
    public class EventsUsers
    {
        public Event Event { get; set; }
        public int EventId { get; set; }
        public User Participant { get; set; }
        public int ParticipantId { get; set; }
    }
}
