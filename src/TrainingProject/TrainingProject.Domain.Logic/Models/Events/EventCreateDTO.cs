using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using TrainingProject.DomainLogic.Annotations;

namespace TrainingProject.DomainLogic.Models.Events
{
    public class EventCreateDTO
    {
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
        [Required]
        [ValidGuid]
        public string OrganizerId { get; set; }
        public string Tags { get; set; }
        [MaxFileSize(8 * 1024 * 1024)]
        [FileType("jpg, jpeg, png")]
        public IFormFile Image { get; set; }
    }
}
