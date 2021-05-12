using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

using System.ComponentModel.DataAnnotations;

using TrainingProject.DomainLogic.Annotations;

namespace TrainingProject.DomainLogic.Models.Events
{
    public sealed class EventUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int ParticipantsLimit { get; set; } //0 - unlimited

        [Required]
        public int? CategoryId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public string Start { get; set; }

        //[Required]
        //public string PublicationEnd { get; set; }

        [Required]
        public string Place { get; set; }

        public string Tags { get; set; }

        [Required]
        public bool IsRecurrent { get; set; }

        [Required]
        public decimal Fee { get; set; }

        [MaxFileSize(8 * 1024 * 1024)]
        [FileType("jpg, jpeg, png")]
        public IFormFile Image { get; set; }

        [Required]
        public bool HasImage { get; set; }

        public string EventDaysOfWeek { get; set; }
    }
}
