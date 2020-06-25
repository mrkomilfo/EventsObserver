using System.ComponentModel.DataAnnotations;

namespace TrainingProject.DomainLogic.Models.Users
{
    public class RefreshDto
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public string RefreshToken { get; set; }
    }
}
