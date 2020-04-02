using System;
using System.Security.Claims;

namespace TrainingProject.DomainLogic.Models.Users
{
    public enum Status
    {
        Ok,
        WrongLoginOrPassword,
        Blocked
    }
    public class LoginResponseDTO
    {
        public string AccessToken { get; set; }
        //public string Name { get; set; }
        //public Claim Role { get; set; }
        public Status Status { get; set; } 
        public DateTime Blocking { get; set; }
    }
}
