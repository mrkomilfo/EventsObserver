namespace TrainingProject.DomainLogic.Models.Users
{
    public class UserToUpdateDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public bool HasPhoto { get; set; }
        public string Photo { get; set; }
    }
}
