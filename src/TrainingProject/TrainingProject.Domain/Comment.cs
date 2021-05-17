using System;

namespace TrainingProject.Domain
{
    public class Comment
    {
        public int Id { get; set; }

        public int EventId { get; set; }

        public Guid? AuthorId { get; set; }

        public string Message { get; set; }

        public DateTime PublicationTime { get; set; }

        public Event Event { get; set; }

        public User Author { get; set; }
    }
}