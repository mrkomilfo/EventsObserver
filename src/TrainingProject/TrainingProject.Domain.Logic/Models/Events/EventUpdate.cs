using System;
using System.Collections.Generic;

namespace TrainingProject.DomainLogic.Models.Events
{
    public class EventUpdate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? CategoryId { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public string Place { get; set; }
        public int Fee { get; set; }
        public int ParticipantsLimit { get; set; } //0 - unlimited
        public ICollection<string> Tags { get; set; }
        public bool HasPhoto { get; set; }
    }
}
