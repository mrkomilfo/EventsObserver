using Microsoft.EntityFrameworkCore;
using TrainingProject.Domain;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace TrainingProject.Data
{
    public sealed class AppContext : DbContext, IAppContext
    {
        public DbSet<Event> Events { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<EventTag> EventsTags { get; set; }

        public DbSet<EventParticipant> EventsParticipants { get; set; }

        public DbSet<EventDayOfWeek> EventDaysOfWeek { get; set; }

        public DbSet<EventDayOfWeekParticipant> RecurrentEventParticipants { get; set; }

        private ModelBuilder _builder;

        public AppContext(DbContextOptions<AppContext> options) : base(options)
        {
            Database.EnsureCreated();

            ChangeTracker.LazyLoadingEnabled = false;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.EnableSensitiveDataLogging();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            _builder = builder;

            ConfigureTag();
            ConfigureUser();
            ConfigureRole();
            ConfigureEvent();
            ConfigureCategory();
            ConfigureEventDayOfWeek();
            ConfigureRecurrentEventParticipant();
        }

        private void ConfigureTag()
        {
            _builder.Entity<Tag>()
                .HasMany(t => t.Events)
                .WithMany(e => e.Tags)
                .UsingEntity<EventTag>(
                    et => et
                        .HasOne(e => e.Event)
                        .WithMany()
                        .HasForeignKey("EventId"),
                    et => et
                        .HasOne(e => e.Tag)
                        .WithMany()
                        .HasForeignKey("TagId"))
                .ToTable("EventsTags")
                .HasKey(et => new {et.EventId, et.TagId});
            _builder.Entity<Tag>()
                .HasKey(t => t.Id);
            _builder.Entity<Tag>()
                .Property(t => t.Id)
                .ValueGeneratedOnAdd();
        }

        private void ConfigureUser()
        {
            _builder.Entity<User>()
                .HasKey(u => u.Id);
            _builder.Entity<User>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd();
            _builder.Entity<User>()
                .HasMany(u => u.OrganizedEvents)
                .WithOne(e => e.Organizer)
                .OnDelete(DeleteBehavior.SetNull);
            _builder.Entity<User>()
                .HasMany(u => u.VisitedRecurrentEvents)
                .WithOne(e => e.Participant);
            _builder.Entity<User>()
                .HasQueryFilter(u => !u.IsDeleted);
            _builder.Entity<User>()
                .HasMany(u => u.VisitedOneTimeEvents)
                .WithMany(e => e.Participants)
                .UsingEntity<EventParticipant>(
                    eu => eu
                        .HasOne(e => e.Event)
                        .WithMany()
                        .HasForeignKey("EventId"),
                    eu => eu
                        .HasOne(e => e.Participant)
                        .WithMany()
                        .HasForeignKey("ParticipantId"))
                .ToTable("EventsUsers")
                .HasKey(eu => new { eu.EventId, eu.ParticipantId });
        }

        private void ConfigureRole()
        {
            _builder.Entity<Role>()
                .HasKey(r => r.Id);
            _builder.Entity<Role>()
                .Property(r => r.Id)
                .ValueGeneratedOnAdd();
        }

        private void ConfigureEvent()
        {
            _builder.Entity<Event>()
                .HasKey(e => e.Id);
            _builder.Entity<Event>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd();
            _builder.Entity<Event>()
                .HasQueryFilter(e => !e.IsDeleted);
            _builder.Entity<Event>()
                .HasMany(e => e.DaysOfWeek)
                .WithOne(e => e.Event)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private void ConfigureCategory()
        {
            _builder.Entity<Category>()
                .HasKey(c => c.Id);
            _builder.Entity<Category>()
                .Property(c => c.Id)
                .ValueGeneratedOnAdd();
            _builder.Entity<Category>()
                .HasQueryFilter(c => !c.IsDeleted);
        }

        private void ConfigureEventDayOfWeek()
        {
            _builder.Entity<EventDayOfWeek>()
                .HasKey(e => e.Id);
            _builder.Entity<EventDayOfWeek>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd();
            _builder.Entity<EventDayOfWeek>()
                .HasMany(e => e.RecurrentEventParticipants)
                .WithOne(e => e.EventDayOfWeek)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private void ConfigureRecurrentEventParticipant()
        {
            _builder.Entity<EventDayOfWeekParticipant>()
                .HasKey(e => e.Id);
            _builder.Entity<EventDayOfWeekParticipant>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd();
        }
    }
}