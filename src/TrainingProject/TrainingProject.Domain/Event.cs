using System;
using System.Collections.Generic;

namespace TrainingProject.Domain
{
    public class Event
    {
        public int Id { get; set; }

        public Guid? OrganizerId { get; set; }

        public int? CategoryId { get; set; }

        public int ParticipantsLimit { get; set; } //0 - unlimited

        public string Name { get; set; }

        public string Description { get; set; }

        public string Place { get; set; }

        public decimal Fee { get; set; }

        public bool HasImage { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsApproved { get; set; }

        public bool IsRecurrent { get; set; }

        public DateTime Start { get; set; }

        public DateTime PublicationTime { get; set; }

        public DateTime PublicationEnd { get; set; }

        public Category Category { get; set; }

        public User Organizer { get; set; }

        public ICollection<User> Participants { get; set; }

        public ICollection<Comment> Comments { get; set; }

        public ICollection<Tag> Tags { get; set; }

        public ICollection<EventDayOfWeek> DaysOfWeek { get; set; }

        public Event()
        {
            Participants = new HashSet<User>();
            Tags = new HashSet<Tag>();
            DaysOfWeek = new HashSet<EventDayOfWeek>();
            Comments = new HashSet<Comment>();
        }
    }
}
