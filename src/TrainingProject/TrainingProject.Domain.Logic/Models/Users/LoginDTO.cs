using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TrainingProject.DomainLogic.Models.Users
{
    public class LoginDto
    {
        [Required]
        public string Login { get; set; }
        [Required]
        public string Password { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
    }
}
