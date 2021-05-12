using System.ComponentModel.DataAnnotations;

using TrainingProject.DomainLogic.Annotations;

namespace TrainingProject.DomainLogic.Models.Users
{
    public class BanDto
    {
        [Required]
        [ValidGuid]
        public string Id { get; set; }

        public int? Days { get; set; }

        public int? Hours { get; set; }
    }
}
