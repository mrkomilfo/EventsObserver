using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingProject.Domain
{
    public class Event
    {
        public Event()
        {
           Participants = new HashSet<User>();
           Tags = new HashSet<Tag>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public byte? CategoryId { get; set; }
        public Category Category { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public string Place { get; set; }
        public int Fee { get; set; }
        public int ParticipantsLimit { get; set; } //0 - unlimited
        public int? OrganizerId { get; set; }
        public User Organizer { get; set; }
        public ICollection<User> Participants { get; set; }
        public ICollection<Tag> Tags { get; set; }
        [NotMapped]
        public string Image { get; set; }
        public DateTime PublicationTime { get; set; }
    }
}
