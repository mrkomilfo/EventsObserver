using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace TrainingProject.DomainLogic.Models.Events
{
    public sealed class EventUpdateDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int? CategoryId { get; set; }
        public string Description { get; set; }
        [Required]
        public string Start { get; set; }
        [Required]
        public string Place { get; set; }
        [Required]
        public decimal Fee { get; set; }
        [Required]
        public int ParticipantsLimit { get; set; } //0 - unlimited
        public string Tags { get; set; }
        public IFormFile Image { get; set; }
        [Required]
        public bool HasImage { get; set; }
    }
}
