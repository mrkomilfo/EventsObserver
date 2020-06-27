using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using TrainingProject.Domain;

namespace TrainingProject.Data
{
    public interface IAppContext
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        public DbSet<Event> Events { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<EventsTags> EventsTags { get; set; }
        public DbSet<EventsUsers> EventsUsers { get; set; }
    }
}
