﻿using System.ComponentModel.DataAnnotations;

namespace TrainingProject.DomainLogic.Models.Users
{
    public class RegisterDto
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string UserName { get; set; }

        public string ContactPhone { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string PasswordConfirm { get; set; }
    }
}
