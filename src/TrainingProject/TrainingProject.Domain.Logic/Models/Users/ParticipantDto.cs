namespace TrainingProject.DomainLogic.Models.Users
{
    public class ParticipantDto
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public string Code { get; set; }

        public bool IsChecked { get; set; }
    }
}