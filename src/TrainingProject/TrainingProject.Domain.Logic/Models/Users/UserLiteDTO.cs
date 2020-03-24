using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingProject.DomainLogic.Models.Users
{
    public class UserLiteDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
    }
}
