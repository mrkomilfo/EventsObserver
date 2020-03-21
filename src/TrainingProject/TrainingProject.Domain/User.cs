using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingProject.Domain
{
    public class User
    {
        public User()
        {
            OrganizedEvents = new HashSet<Event>();
        }

        public int Id { get; set; }
        public string UserName { get; set; }
        [Index("INDEX_LOGIN", IsClustered = true, IsUnique = true)]
        public string Login { get; set; }
        public string Password { get; set; }
        public byte? RoleId { get; set; }
        public Role Role { get; set; }
        public DateTime? UnlockTime { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        [NotMapped]
        public string Photo { get; set; }
        public DateTime RegistrationDate { get; set; }
        public virtual ICollection<Event> OrganizedEvents { get; set; }
        public virtual ICollection<Event> VisitedEvents { get; set; }
    }
}
