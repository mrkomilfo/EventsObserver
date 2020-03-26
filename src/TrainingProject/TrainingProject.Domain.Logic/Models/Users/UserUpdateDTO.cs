using Microsoft.AspNetCore.Http;

namespace TrainingProject.DomainLogic.Models.Users
{
    public class UserUpdateDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public bool HasPhoto { get; set; }
        public IFormFile Photo { get; set; }
    }
}
