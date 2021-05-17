using System;
using System.Collections.Generic;

namespace TrainingProject.DomainLogic.Models.Events
{
    public class EventToUpdateDto
    {
        public int Id { get; set; }

        public int? CategoryId { get; set; }

        public int ParticipantsLimit { get; set; } //0 - unlimited

        public decimal Fee { get; set; }

        public bool IsRecurrent { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Start { get; set; }

        public string PublicationEnd { get; set; }

        public string Place { get; set; }

        public string Tags { get; set; }

        public string Image { get; set; }

        public string WeekDays { get; set; }
    }
}
