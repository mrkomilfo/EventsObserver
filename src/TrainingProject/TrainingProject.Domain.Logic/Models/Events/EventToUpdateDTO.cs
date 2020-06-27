namespace TrainingProject.DomainLogic.Models.Events
{
    public class EventToUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? CategoryId { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Place { get; set; }
        public decimal Fee { get; set; }
        public int ParticipantsLimit { get; set; } //0 - unlimited
        public string Tags { get; set; }
        public string Image { get; set; }
    }
}
