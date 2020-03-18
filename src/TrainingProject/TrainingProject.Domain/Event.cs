using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingProject.Domain
{
    public class Event
    {
        public int EventId { get; set; }
        public string Name { get; set; }
        public Category Category { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public string Place { get; set; }
        public int Fee { get; set; }
        public int ParticipantsLimit { get; set; } //0 - unlimited
        public User Organizer { get; set; }
        public virtual IEnumerable<User> Participants { get; set; }
        public virtual IEnumerable<Tag> Tags { get; set; }
        [NotMapped]
        public string Image { get; set; }
        public DateTime PublicationTime { get; set; }
    }
}
