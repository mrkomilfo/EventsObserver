using System.Security.Claims;

namespace TrainingProject.DomainLogic.Models.Users
{
    public class LoginResponseDTO
    {
        public string AccessToken { get; set; }
        public string Name { get; set; }
        public Claim Role { get; set; }
    }
}
