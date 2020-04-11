using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using TrainingProject.DomainLogic.Annotations;

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
        public decimal Fee { get; set; }
        [Required]
        public int ParticipantsLimit { get; set; } //0 - unlimited
        [Required]
        [ValidGuid]
        public string OrganizerId { get; set; }
        public ICollection<string> Tags { get; set; }
        public IFormFile Image { get; set; }
    }
}
