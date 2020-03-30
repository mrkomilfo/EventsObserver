﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TrainingProject.Domain;

namespace TrainingProject.Data
{
    public static class DBInitializer
    {
        public static async Task InitializeEvents(IAppContext context)
        {
            if (!await context.Categories.AnyAsync())
            {
                IEnumerable<Category> categories = new List<Category>()
                {
                    new Category()
                    {
                        Name = "Другое"
                    },
                    new Category()
                    {
                        Name = "Лекции",
                        Description =
                            "В данном разделе размещается информация о проводимых открытых лекциях в нашем городе."
                    },
                    new Category()
                    {
                        Name = "Тренинги",
                        Description =
                            "Активисты из различных общественных организаций регулярно проводят тренинги на социальные и общеобразовательные темы."
                    },
                    
                };
                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync(default);

                IEnumerable<Tag> tags = new List<Tag>()
                {
                    new Tag() { Name = "психология"},
                    new Tag() { Name = "привязанности"},
                    new Tag() { Name = "экология"},
                    new Tag() { Name = "климат"}
                };
                await context.Tags.AddRangeAsync(tags);
                await context.SaveChangesAsync(default);

                IEnumerable<Event> events = new List<Event>()
                {
                    new Event()
                    {
                        Name = "Лекция по психологии",
                        CategoryId = categories.ElementAtOrDefault(1)?.Id,
                        Description = "Лекция на тему привязанностей от магистранта факультета психологии БГУ",
                        Place = "Октябрьская 16",
                        Start = new DateTime(2020, 03, 20, 15, 0, 0),
                        Fee = 0,
                        ParticipantsLimit = 20,
                        OrganizerId = context.Users.Select(u => u.Id).FirstOrDefault(),
                        PublicationTime = new DateTime(2020, 03, 10, 15, 0, 0),
                    },
                    new Event()
                    {
                        Name = "Лекция по экологии",
                        CategoryId = categories.ElementAtOrDefault(1)?.Id,
                        Description = "Лекция на тему изменения климата",
                        Place = "Октябрьская 16",
                        Start = new DateTime(2020, 03, 30, 15, 0, 0),
                        Fee = 0,
                        ParticipantsLimit = 20,
                        OrganizerId = context.Users.Select(u => u.Id).FirstOrDefault(),
                        PublicationTime = DateTime.Now,
                    }
                };
                await context.Events.AddRangeAsync(events);
                await context.SaveChangesAsync(default);

                IEnumerable<EventsTags> eventTags = new List<EventsTags>()
                {
                    new EventsTags{EventId = (int)events.ElementAtOrDefault(0)?.Id, TagId = (int)tags.ElementAtOrDefault(0)?.Id},
                    new EventsTags{EventId = (int)events.ElementAtOrDefault(0)?.Id, TagId = (int)tags.ElementAtOrDefault(1)?.Id},
                    new EventsTags{EventId = (int)events.ElementAtOrDefault(1)?.Id, TagId = (int)tags.ElementAtOrDefault(2)?.Id},
                    new EventsTags{EventId = (int)events.ElementAtOrDefault(1)?.Id, TagId = (int)tags.ElementAtOrDefault(3)?.Id},
                };
                context.EventsTags.AddRange(eventTags);
                await context.SaveChangesAsync(default);
            }
        }

        public static async Task InitializeUsers(AppContext context)
        {
            if (!(await context.Roles.AnyAsync() || await context.Users.AnyAsync()))
            {
                IEnumerable<Role> roles = new List<Role>()
                {
                    new Role { Name = "Account manager"},
                    new Role { Name = "Admin"},
                    new Role { Name = "User"},
                }; 
                context.Roles.AddRange(roles);
                await context.SaveChangesAsync();

                IEnumerable<User> users = new List<User>()
                {
                    new User
                    {
                        Login = "accountManager",
                        Password = "YIMMU3jl8cYYVN8TGTunenCKof4NfTmY8D0/quh0WU4=",
                        UserName = "AccountManager",
                        RoleId = roles.ElementAtOrDefault(0)?.Id,
                        RegistrationDate = DateTime.Now,
                    },
                    new User
                    {
                        Login = "admin",
                        Password = "jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=",
                        UserName = "Administrator",
                        RoleId = roles.ElementAtOrDefault(1)?.Id,
                        RegistrationDate = DateTime.Now,
                    },
                    new User
                    {
                        Login = "user",
                        Password = "BPiZbadjt6lpsQKO4wB1aerzpjVIbdqyEdUSyFud+Ps=",
                        UserName = "User",
                        RoleId = roles.ElementAtOrDefault(2)?.Id,
                        RegistrationDate = DateTime.Now,
                    },
                };
                context.Users.AddRange(users);
                await context.SaveChangesAsync();
            }
        }
    }
}
