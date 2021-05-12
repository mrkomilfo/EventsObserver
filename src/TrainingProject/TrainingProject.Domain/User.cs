using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingProject.Domain
{
    public sealed class User
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        [Index("INDEX_LOGIN", IsClustered = true, IsUnique = true)]
        public string Login { get; set; }

        public string Password { get; set; }

        public int? RoleId { get; set; }

        public Role Role { get; set; }

        public DateTime? UnlockTime { get; set; }

        public string ContactEmail { get; set; }

        public string ContactPhone { get; set; }

        public DateTime RegistrationDate { get; set; }

        public bool HasPhoto { get; set; }

        public string RefreshToken { get; set; }

        public bool IsDeleted { get; set; }

        public bool EmailConfirmed { get; set; }

        public string EmailConfirmCodeHash { get; set; }

        public string PasswordResetCodeHash { get; set; }

        public ICollection<Event> OrganizedEvents { get; set; }

        public ICollection<Event> VisitedOneTimeEvents { get; set; }

        public ICollection<EventDayOfWeekParticipant> VisitedRecurrentEvents { get; set; }

        public User()
        {
            OrganizedEvents = new HashSet<Event>();
            VisitedOneTimeEvents = new HashSet<Event>();
            VisitedRecurrentEvents = new HashSet<EventDayOfWeekParticipant>();
        }
    }
}
