using System.ComponentModel.DataAnnotations;
using TrainingProject.DomainLogic.Annotations;

namespace TrainingProject.DomainLogic.Models.Users
{
    public class UserToBanDTO
    {
        [ValidGuid]
        [Required]
        public string Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public bool IsBanned { get; set; }
    }
}
