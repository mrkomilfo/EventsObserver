using System;
using System.Collections.Generic;

namespace TrainingProject.DomainLogic.Models.Events
{
    public class EventFull
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? CategoryId { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public string Place { get; set; }
        public string Fee { get; set; }
        public int ParticipantsLimit { get; set; } //0 - unlimited
        public int? OrganizerId { get; set; }
        public string Organizer { get; set; }
        public ICollection<Dictionary<int, string>> Participants { get; set; }
        public ICollection<string> Tags { get; set; }
        public DateTime PublicationTime { get; set; }
        public string Photo { get; set; }
    }
}
