﻿using System;

namespace TrainingProject.DomainLogic.Models.Users
{
    public class UserFullDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public DateTime RegistrationDate { get; set; }
        public int OrganizedEvents { get; set; }
        public int VisitedEvents { get; set; }
        public string Photo { get; set; }
    }
}
