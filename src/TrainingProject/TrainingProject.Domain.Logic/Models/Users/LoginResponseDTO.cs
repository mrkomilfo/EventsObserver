namespace TrainingProject.DomainLogic.Models.Users
{
    public class LoginResponseDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
    }
}
