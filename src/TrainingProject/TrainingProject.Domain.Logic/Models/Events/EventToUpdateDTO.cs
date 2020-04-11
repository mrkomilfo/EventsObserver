using System;
using System.Collections.Generic;

namespace TrainingProject.DomainLogic.Models.Events
{
    public class EventToUpdateDTO
    {
        public EventToUpdateDTO()
        {
            Tags = new HashSet<string>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public int? CategoryId { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public string Place { get; set; }
        public decimal Fee { get; set; }
        public int ParticipantsLimit { get; set; } //0 - unlimited
        public ICollection<string> Tags { get; set; }
        public string Image { get; set; }
        public bool HasImage { get; set; }
    }
}
