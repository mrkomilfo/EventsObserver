namespace TrainingProject.Domain
{
    public class EventTag
    {
        public int EventId { get; set; }

        public int TagId { get; set; }

        public Event Event { get; set; }

        public Tag Tag { get; set; }
    }
}
