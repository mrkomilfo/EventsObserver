using System;
using System.ComponentModel.DataAnnotations;

namespace TrainingProject.DomainLogic.Models.Users
{
    public class BanDTO
    {
        public string UserId { get; set; }
        public int? Days { get; set; }
        public int? Hours { get; set; }
    }
}
