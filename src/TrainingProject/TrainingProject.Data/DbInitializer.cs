using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrainingProject.Domain;

namespace TrainingProject.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeDb(AppContext context)
        {
            if (!context.Roles.Any())
            {
                await InitializeUsersAsync(context);
                await InitializeEventsAsync(context);
                await InitializeCommentsAsync(context);
            }
        }

        private static async Task InitializeUsersAsync(AppContext context)
        {
            IEnumerable<Role> roles = new List<Role>
            {
                new Role { Name = "Account manager"},
                new Role { Name = "Admin"},
                new Role { Name = "User"},
            };
            context.Roles.AddRange(roles);
            await context.SaveChangesAsync();

            IEnumerable<User> users = new List<User>
            {
                new User
                {
                    Email = "accountManager@gmail.com",
                    Password = "YIMMU3jl8cYYVN8TGTunenCKof4NfTmY8D0/quh0WU4=", //accountManager
                    UserName = "AccountManager",
                    RoleId = roles.ElementAt(0).Id,
                    RegistrationDate = DateTime.Now
                },
                new User
                {
                    Email = "admin@gmail.com",
                    Password = "jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=", //admin
                    UserName = "Administrator",
                    RoleId = roles.ElementAt(1).Id,
                    RegistrationDate = DateTime.Now
                },
                new User
                {
                    Email = "user@gmail.com",
                    Password = "BPiZbadjt6lpsQKO4wB1aerzpjVIbdqyEdUSyFud+Ps=", //user
                    UserName = "User",
                    RoleId = roles.ElementAt(2).Id,
                    RegistrationDate = DateTime.Now
                }
            };
            context.Users.AddRange(users);
            await context.SaveChangesAsync();
        }

        private static async Task InitializeEventsAsync(IAppContext context)
        {
            IEnumerable<Category> categories = new List<Category>
            {
                new Category
                {
                    Name = "Другое"
                },
                new Category
                {
                    Name = "Лекции",
                    Description =
                        "В данном разделе размещается информация о проводимых открытых лекциях в нашем городе."
                },
                new Category
                {
                    Name = "Тренинги",
                    Description =
                        "Активисты из различных общественных организаций регулярно проводят тренинги на социальные и общеобразовательные темы."
                },
                new Category
                {
                    Name = "Концерты",
                    Description = "Выступления местных и зарубежных исполнителей."
                }
            };
            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync(default);

            IEnumerable<Tag> tags = new List<Tag>
            {
                new Tag { Name = "психология"},
                new Tag { Name = "привязанности"},
                new Tag { Name = "экология"},
                new Tag { Name = "климат"}
            };
            await context.Tags.AddRangeAsync(tags);
            await context.SaveChangesAsync(default);

            IEnumerable<Event> events = new List<Event>
            {
                new Event
                {
                    Name = "Лекция по психологии",
                    CategoryId = categories.ElementAt(1).Id,
                    Description = "Лекция на тему привязанностей от магистранта факультета психологии БГУ",
                    Place = "Октябрьская 16",
                    Start = DateTime.Now.AddDays(30),
                    Fee = 0,
                    ParticipantsLimit = 20,
                    OrganizerId = context.Users.Select(u => u.Id).FirstOrDefault(),
                    PublicationTime = new DateTime(2020, 03, 10, 15, 0, 0),
                    PublicationEnd = new DateTime(2021, 06, 30, 15, 0, 0),
                    HasImage = true,
                    IsApproved = true
                },
                new Event
                {
                    Name = "Лекция по экологии",
                    CategoryId = categories.ElementAt(1).Id,
                    Description = "Лекция на тему изменения климата",
                    Place = "Октябрьская 16",
                    Start = DateTime.Now.AddDays(30),
                    Fee = 0,
                    ParticipantsLimit = 20,
                    OrganizerId = context.Users.Select(u => u.Id).FirstOrDefault(),
                    PublicationTime = DateTime.Now,
                    PublicationEnd = new DateTime(2021, 06, 30, 15, 0, 0),
                    HasImage = true,
                    IsApproved = true
                }
            };
            await context.Events.AddRangeAsync(events);
            await context.SaveChangesAsync(default);

            IEnumerable<EventTag> eventTags = new List<EventTag>
            {
                new EventTag
                {
                    EventId = events.ElementAt(0).Id,
                    TagId = tags.ElementAt(0).Id
                },
                new EventTag
                {
                    EventId = events.ElementAt(0).Id,
                    TagId = tags.ElementAt(1).Id
                },
                new EventTag
                {
                    EventId = events.ElementAt(1).Id,
                    TagId = tags.ElementAt(2).Id
                },
                new EventTag
                {
                    EventId = events.ElementAt(1).Id,
                    TagId = tags.ElementAt(3).Id
                },
            };
            context.EventsTags.AddRange(eventTags);
            await context.SaveChangesAsync(default);

            IEnumerable<EventParticipant> eventUsers = new List<EventParticipant>
            {
                new EventParticipant
                {
                    EventId = events.ElementAt(0).Id,
                    ParticipantId = context.Users.Where(u => string.Equals(u.Email, "admin@gmail.com"))
                        .Select(u => u.Id).FirstOrDefault(),
                    Code = "2377"
                },
                new EventParticipant
                {
                    EventId = events.ElementAt(1).Id,
                    ParticipantId = context.Users.Where(u => string.Equals(u.Email, "user@gmail.com"))
                        .Select(u => u.Id).FirstOrDefault(),
                    Code = "7342"
                }
            };
            context.EventsParticipants.AddRange(eventUsers);
            await context.SaveChangesAsync(default);
        }

        private static async Task InitializeCommentsAsync(AppContext context)
        {
            IEnumerable<Comment> comments = new List<Comment>
            {
                new Comment
                {
                    EventId = context.Events.ToList().ElementAt(0).Id,
                    AuthorId = context.Users.ToList().ElementAt(1).Id,
                    Message = "Cool",
                    PublicationTime = DateTime.Now
                },
                new Comment
                {
                    EventId = context.Events.ToList().ElementAt(1).Id,
                    AuthorId = context.Users.ToList().ElementAt(0).Id,
                    Message = "Awesome",
                    PublicationTime = DateTime.Now
                }
            };
            context.Comments.AddRange(comments);
            await context.SaveChangesAsync();
        }
    }
}
