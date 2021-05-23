namespace TrainingProject.DomainLogic.Models.Events
{
    public class EventDayOfWeekDto
    {
        public int Id { get; set; }

        public int WeekDay { get; set; }

        public string Date { get; set; }

        public string Time { get; set; }

        public int Participants { get; set; }

        public bool AmISubscribed { get; set; }
    }
}