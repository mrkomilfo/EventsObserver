using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingProject.Domain
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        [Index("INDEX_LOGIN", IsClustered = true, IsUnique = true)]
        public string Login { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; }
        public DateTime? UnlockTime { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        [NotMapped]
        public string Photo { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}
