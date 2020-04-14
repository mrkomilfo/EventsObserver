 using System;
using System.Collections.Generic;

namespace TrainingProject.Domain
{
    public sealed class Event
    {
        public Event()
        {
            Participants = new HashSet<User>();
            Tags = new HashSet<Tag>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int? CategoryId { get; set; }
        public Category Category { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public string Place { get; set; }
        public decimal Fee { get; set; }
        public int ParticipantsLimit { get; set; } //0 - unlimited
        public Guid? OrganizerId { get; set; }
        public User Organizer { get; set; }
        public ICollection<User> Participants { get; set; }
        public ICollection<Tag> Tags { get; set; }
        public DateTime PublicationTime { get; set; }
        public bool HasImage { get; set; }
        public bool IsDeleted { get; set; }
    }
}
