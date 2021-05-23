using System.Collections.Generic;

namespace TrainingProject.DomainLogic.Models.Events
{
    public sealed class EventFullDto
    {
        public int Id { get; set; }

        public int? CategoryId { get; set; }

        public long StartParsable { get; set; }

        public decimal Fee { get; set; }

        public string Place { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public string Description { get; set; }

        public string Start { get; set; }

        public string OrganizerId { get; set; }

        public string Organizer { get; set; }

        public string PublicationTime { get; set; }

        public string PublicationEnd { get; set; }

        public string Image { get; set; }

        public bool IsRecurrent { get; set; }

        public int ParticipantsLimit { get; set; } //0 - unlimited

        public int Participants { get; set; }

        public bool AmISubscribed { get; set; }

        public bool IsActive { get; set; }

        public Dictionary<string, string> Tags { get; set; }

        public IEnumerable<EventDayOfWeekDto> WeekDays { get; set; }

        public EventFullDto()
        {
            Tags = new Dictionary<string, string>();
            WeekDays = new List<EventDayOfWeekDto>();
        }
    }
}
