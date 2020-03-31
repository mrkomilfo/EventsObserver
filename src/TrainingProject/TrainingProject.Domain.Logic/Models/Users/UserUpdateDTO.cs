using Microsoft.AspNetCore.Http;
using System;

namespace TrainingProject.DomainLogic.Models.Users
{
    public class UserUpdateDTO
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public bool HasPhoto { get; set; }
        public IFormFile Photo { get; set; }
    }
}
