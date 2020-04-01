using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace TrainingProject.DomainLogic.Models.Events
{
    public class EventCreateDTO
    {
        public EventCreateDTO()
        {
            Tags = new HashSet<string>();
        }
        [Required]
        public string Name { get; set; }
        [Required]
        public int? CategoryId { get; set; }
        public string Description { get; set; }
        [Required]
        public DateTime Start { get; set; }
        [Required]
        public string Place { get; set; }
        [Required]
        public int Fee { get; set; }
        [Required]
        public int ParticipantsLimit { get; set; } //0 - unlimited
        [Required]
        public int? OrganizerId { get; set; }
        public ICollection<string> Tags { get; set; }
        public IFormFile Image { get; set; }
    }
}
