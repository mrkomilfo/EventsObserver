using System;
using System.Globalization;
using AutoMapper;
using TrainingProject.Domain;
using TrainingProject.DomainLogic.Models.Categories;
using TrainingProject.DomainLogic.Models.Events;
using TrainingProject.DomainLogic.Models.Users;

namespace TrainingProject.DomainLogic.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CategoryCreateDTO, Category>();
            CreateMap<Category, CategoryLiteDTO>();
            CreateMap<Category, CategoryFullDTO>();

            CreateMap<EventCreateDTO, Event>()
                .ForMember(m => m.Start, opt => opt.MapFrom(m => DateTime.ParseExact(m.Start, "d/M/yyyy HH:mm", CultureInfo.InvariantCulture)))
                .ForMember(m => m.OrganizerId, opt => opt.MapFrom(m => Guid.Parse(m.OrganizerId)))
                .ForMember(m => m.HasImage, opt => opt.MapFrom(m => m.Image != null))
                .ForMember(m => m.Tags, opt=>opt.Ignore())
                .ForMember(m => m.PublicationTime, opt => opt.MapFrom(m => DateTime.Now));
            CreateMap<string, Tag>()
                .ForMember(m => m.Name, opt => opt.MapFrom(m => m.ToLower()));
            CreateMap<Tag, string>()
               .ConvertUsing(source => source.Name ?? string.Empty);
            CreateMap<EventUpdateDTO, Event>()
                .ForMember(m => m.Start, opt => opt.MapFrom(m => DateTime.ParseExact(m.Start, "d/M/yyyy HH:mm", CultureInfo.InvariantCulture)))
                .ForMember(m => m.Tags, opt => opt.Ignore());
            CreateMap<Event, EventToUpdateDTO>()
                .ForMember(m => m.Tags, opt => opt.Ignore())
                .ForMember(m => m.Time, opt => opt.MapFrom(m => m.Start.ToString("HH:mm")))
                .ForMember(m => m.Date, opt => opt.MapFrom(m => m.Start.ToString("yyyy-MM-dd")));
            CreateMap<Event, EventFullDTO>()
                .ForMember(m => m.Category, opt => opt.MapFrom(m => m.Category.Name))
                .ForMember(m => m.Organizer, opt => opt.MapFrom(m => m.Organizer.UserName))
                .ForMember(m => m.OrganizerId, opt => opt.MapFrom(m => m.Organizer.Id.ToString()))
                .ForMember(m => m.Start, opt => opt.MapFrom(m => m.Start.ToString("f")))
                .ForMember(m => m.PublicationTime, opt => opt.MapFrom(m => m.PublicationTime.ToString("f")))
                .ForMember(m => m.Tags, opt => opt.Ignore());
            CreateMap<Event, EventLiteDTO>()
                .ForMember(m => m.Category, opt => opt.MapFrom(m => m.Category.Name))
                .ForMember(m => m.Start, opt => opt.MapFrom(m => m.Start.ToString("f")));

            CreateMap<User, UserFullDTO>()
                .ForMember(m => m.Id, opt => opt.MapFrom(m => m.Id.ToString()))
                .ForMember(m => m.Role, opt => opt.MapFrom(m => m.Role.Name))
                .ForMember(m => m.Status, opt => opt.MapFrom(m => m.UnlockTime == null || m.UnlockTime < DateTime.Now ? null : $"Заблокирован до {m.UnlockTime}"))
                .ForMember(m => m.RegistrationDate, opt => opt.MapFrom(m => m.RegistrationDate.ToString("f")))
                .ForMember(m => m.OrganizedEvents, opt => opt.MapFrom(m => m.OrganizedEvents.Count))
                .ForMember(m => m.VisitedEvents, opt => opt.Ignore());
            CreateMap<User, UserLiteDTO>()
                .ForMember(m => m.Id, opt => opt.MapFrom(m => m.Id.ToString()))
                .ForMember(m => m.Role, opt => opt.MapFrom(m => m.Role.Name))
                .ForMember(m => m.Status, opt => opt.MapFrom(m => m.UnlockTime == null || m.UnlockTime < DateTime.Now ? null : $"Заблокирован до {m.UnlockTime}"));
            CreateMap<User, ChangeRoleDTO>()
                .ForMember(m => m.UserId, opt => opt.MapFrom(m => m.Id.ToString()));
            CreateMap<User, UserToBanDTO>()
                .ForMember(m => m.Id, opt => opt.MapFrom(m => m.Id.ToString()))
                .ForMember(m => m.IsBanned, opt => opt.MapFrom(m => m.UnlockTime != null && m.UnlockTime > DateTime.Now));
            CreateMap<UserUpdateDTO, User>()
                .ForMember(m => m.Id, opt => opt.MapFrom(m => Guid.Parse(m.Id)));
            CreateMap<User, UserToUpdateDTO>()
                .ForMember(m => m.Id, opt => opt.MapFrom(m => m.Id.ToString()));
            CreateMap<User, UserRoleDTO>()
                .ForMember(m => m.UserId, opt => opt.MapFrom(m => m.Id.ToString()));
            CreateMap<RegisterDTO, User>()
                .ForMember(m => m.Password, opt => opt.MapFrom(m => HashGenerator.Encrypt(m.Password)))
                .ForMember(m => m.RegistrationDate, opt => opt.MapFrom(m => DateTime.Now));
        }
    }
}