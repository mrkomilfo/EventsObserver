﻿using System.Data.Entity;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TrainingProject.Domain;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace TrainingProject.Data
{
    public sealed class AppContext : DbContext, IAppContext
    {
        public AppContext(DbContextOptions<AppContext> options) : base(options)
        {
            Database.EnsureCreated();
            this.ChangeTracker.LazyLoadingEnabled = false;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder
                //Log parameter values
                .EnableSensitiveDataLogging();

        public Microsoft.EntityFrameworkCore.DbSet<Event> Events { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<Category> Categories { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<Tag> Tags { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<User> Users { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<Role> Roles { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<EventsTags> EventsTags { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<EventsUsers> EventsUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Tag>()
                .HasMany(t=>t.Events)
                .WithMany(e => e.Tags)
                .UsingEntity<EventsTags>(
                    et => et
                        .HasOne(e => e.Event)
                        .WithMany()
                        .HasForeignKey("EventId"),
                    et => et
                        .HasOne(e => e.Tag)
                        .WithMany()
                        .HasForeignKey("TagId"))
                .ToTable("EventsTags")
                .HasKey(et => new { et.EventId, et.TagId });

            builder.Entity<User>()
                .HasMany(u => u.VisitedEvents)
                .WithMany(e => e.Participants)
                .UsingEntity<EventsUsers>(
                    eu=>eu
                        .HasOne(e=>e.Event)
                        .WithMany()
                        .HasForeignKey("EventId"),
                    eu => eu
                        .HasOne(e => e.Participant)
                        .WithMany()
                        .HasForeignKey("ParticipantId"))
                .ToTable("EventsUsers")
                .HasKey(eu=>new { eu.EventId, eu.ParticipantId});

            builder.Entity<User>()
                .HasKey(u => u.Id);

            builder.Entity<User>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd();

            builder.Entity<User>()
                .HasMany(u => u.OrganizedEvents)
                .WithOne(e => e.Organizer)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<User>()
                .HasQueryFilter(u => !u.IsDeleted);


            builder.Entity<Role>()
                .HasKey(r => r.Id);

            builder.Entity<Role>()
                .Property(r => r.Id)
                .ValueGeneratedOnAdd();


            builder.Entity<Event>()
                .HasKey(e => e.Id);

            builder.Entity<Event>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd();
            builder.Entity<Event>()
                .HasQueryFilter(e => !e.IsDeleted);


            builder.Entity<Category>()
                .HasKey(c => c.Id);

            builder.Entity<Category>()
                .Property(c => c.Id)
                .ValueGeneratedOnAdd();

            builder.Entity<Category>()
                .HasQueryFilter(c => !c.IsDeleted);


            builder.Entity<Tag>()
                .HasKey(t => t.Id);

            builder.Entity<Tag>()
                .Property(t => t.Id)
                .ValueGeneratedOnAdd();
        }
    }
}