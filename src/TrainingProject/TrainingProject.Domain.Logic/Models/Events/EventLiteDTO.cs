using System;
using System.Collections.Generic;

namespace TrainingProject.DomainLogic.Models.Events
{
    public sealed class EventLiteDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? CategoryId { get; set; }
        public string Category { get; set; }
        public string Start { get; set; }
        public string Place { get; set; }
        public decimal Fee { get; set; } //0 - unlimited
        public bool HasImage { get; set; }
        public string Image { get; set; }
    }
}
