using System.ComponentModel.DataAnnotations;

namespace TrainingProject.DomainLogic.Models.Users
{
    public class LoginDTO
    { 
        [Required]
        public string Login { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
