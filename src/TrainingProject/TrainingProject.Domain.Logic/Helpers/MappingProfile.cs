using AutoMapper;

using System;
using System.Globalization;

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
            CreateMap<CategoryCreateDto, Category>();
            CreateMap<Category, CategoryFullDto>();
            CreateMap<Category, CategoryLiteDto>();

            CreateMap<EventCreateDto, Event>()
                .ForMember(m => m.Start,
                    opt => opt.MapFrom(m => string.IsNullOrEmpty(m.Start) ? new DateTime() : DateTime.ParseExact(m.Start, "d/M/yyyy H:m", CultureInfo.InvariantCulture)))
                .ForMember(m => m.OrganizerId, opt => opt.MapFrom(m => Guid.Parse(m.OrganizerId)))
                .ForMember(m => m.HasImage, opt => opt.MapFrom(m => m.Image != null))
                .ForMember(m => m.Tags, opt => opt.Ignore())
                .ForMember(m => m.PublicationTime, opt => opt.MapFrom(m => DateTime.Now));
            CreateMap<string, Tag>()
                .ForMember(m => m.Name, opt => opt.MapFrom(m => m.ToLower()));
            CreateMap<Tag, string>()
               .ConvertUsing(source => source.Name ?? string.Empty);
            CreateMap<EventUpdateDto, Event>()
                .ForMember(m => m.Start, opt => opt.MapFrom(m => DateTime.ParseExact(m.Start, "d/M/yyyy H:m", CultureInfo.InvariantCulture)))
                .ForMember(m => m.Tags, opt => opt.Ignore());
            CreateMap<Event, EventToUpdateDto>()
                .ForMember(m => m.Tags, opt => opt.Ignore())
                .ForMember(m => m.DateTime, opt => opt.MapFrom(m => m.Start.ToString("yyyy-MM-ddTHH:mm")));
            CreateMap<Event, EventFullDto>()
                .ForMember(m => m.Category, opt => opt.MapFrom(m => m.Category.Name))
                .ForMember(m => m.Organizer, opt => opt.MapFrom(m => m.Organizer.UserName))
                .ForMember(m => m.OrganizerId, opt => opt.MapFrom(m => m.Organizer.Id.ToString()))
                .ForMember(m => m.Start, opt => opt.MapFrom(m => m.Start.ToString("f")))
                .ForMember(m => m.StartParsable, opt => opt.MapFrom(m => m.Start.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds))
                .ForMember(m => m.PublicationTime, opt => opt.MapFrom(m => m.PublicationTime.ToString("f")))
                .ForMember(m => m.Tags, opt => opt.Ignore());
            CreateMap<Event, EventLiteDto>()
                .ForMember(m => m.Category, opt => opt.MapFrom(m => m.Category.Name))
                .ForMember(m => m.Start, opt => opt.MapFrom(m => m.Start.ToString("f")));

            CreateMap<User, UserFullDto>()
                .ForMember(m => m.Id, opt => opt.MapFrom(m => m.Id.ToString()))
                .ForMember(m => m.Role, opt => opt.MapFrom(m => m.Role.Name))
                .ForMember(m => m.Status, opt => opt.MapFrom(m => m.UnlockTime == null || m.UnlockTime < DateTime.Now ? null : $"Заблокирован до {m.UnlockTime}"))
                .ForMember(m => m.RegistrationDate, opt => opt.MapFrom(m => m.RegistrationDate.ToString("f")))
                .ForMember(m => m.OrganizedEvents, opt => opt.MapFrom(m => m.OrganizedEvents.Count))
                .ForMember(m => m.VisitedEvents, opt => opt.Ignore());
            CreateMap<User, UserLiteDto>()
                .ForMember(m => m.Id, opt => opt.MapFrom(m => m.Id.ToString()))
                .ForMember(m => m.Role, opt => opt.MapFrom(m => m.Role.Name))
                .ForMember(m => m.Status, opt => opt.MapFrom(m => m.UnlockTime == null || m.UnlockTime < DateTime.Now ? null : $"Заблокирован до {m.UnlockTime}"));
            CreateMap<User, ChangeRoleDto>()
                .ForMember(m => m.UserId, opt => opt.MapFrom(m => m.Id.ToString()));
            CreateMap<User, UserToBanDto>()
                .ForMember(m => m.Id, opt => opt.MapFrom(m => m.Id.ToString()))
                .ForMember(m => m.IsBanned, opt => opt.MapFrom(m => m.UnlockTime != null && m.UnlockTime > DateTime.Now));
            CreateMap<UserUpdateDto, User>()
                .ForMember(m => m.Id, opt => opt.MapFrom(m => Guid.Parse(m.Id)));
            CreateMap<User, UserToUpdateDto>()
                .ForMember(m => m.Id, opt => opt.MapFrom(m => m.Id.ToString()));
            CreateMap<User, UserRoleDto>()
                .ForMember(m => m.UserId, opt => opt.MapFrom(m => m.Id.ToString()));
            CreateMap<RegisterDto, User>()
                .ForMember(m => m.Password, opt => opt.MapFrom(m => HashGenerator.Encrypt(m.Password)))
                .ForMember(m => m.RegistrationDate, opt => opt.MapFrom(m => DateTime.Now));
        }
    }
}