using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace TrainingProject.DomainLogic.Models.Users
{
    public class UserUpdateDTO
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string UserName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        [Required]
        public bool HasPhoto { get; set; }
        public IFormFile Photo { get; set; }
    }
}
