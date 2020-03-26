using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace TrainingProject.DomainLogic.Models.Events
{
    public class EventCreateDTO
    {
        public EventCreateDTO()
        {
            Tags = new HashSet<string>();
        }

        public string Name { get; set; }
        public int? CategoryId { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public string Place { get; set; }
        public int Fee { get; set; }
        public int ParticipantsLimit { get; set; } //0 - unlimited
        public int? OrganizerId { get; set; }
        public ICollection<string> Tags { get; set; }
        public IFormFile Image { get; set; }
    }
}
