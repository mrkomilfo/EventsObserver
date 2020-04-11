﻿using System;
using System.Collections.Generic;

namespace TrainingProject.DomainLogic.Models.Events
{
    public sealed class EventFullDTO
    {
        public EventFullDTO()
        {
            Tags = new HashSet<string>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public int? CategoryId { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public string Place { get; set; }
        public decimal Fee { get; set; }
        public int ParticipantsLimit { get; set; } //0 - unlimited
        public string OrganizerId { get; set; }
        public string Organizer { get; set; }
        public Dictionary<string, string> Participants { get; set; }
        public ICollection<string> Tags { get; set; }
        public DateTime PublicationTime { get; set; }
        public string Image { get; set; }
    }
}
