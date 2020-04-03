using System;
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
                .ForMember(m => m.OrganizerId, opt => opt.MapFrom(m => Guid.Parse(m.OrganizerId)))
                .ForMember(m => m.HasPhoto, opt => opt.MapFrom(m => m.Image != null))
                .ForMember(m => m.PublicationTime, opt => opt.MapFrom(m => DateTime.Now));
            CreateMap<string, Tag>()
                .ForMember(m => m.Name, opt => opt.MapFrom(m => m));
            CreateMap<EventUpdateDTO, Event>();
            CreateMap<Event, EventToUpdateDTO>();
            CreateMap<Event, EventFullDTO>()
                .ForMember(m => m.Category, opt => opt.MapFrom(m => m.Category.Name))
                .ForMember(m => m.Organizer, opt => opt.MapFrom(m => m.Organizer.UserName))
                .ForMember(m => m.OrganizerId, opt => opt.MapFrom(m => m.Organizer.Id.ToString()));
            CreateMap<Event, EventLiteDTO>()
                .ForMember(m => m.Category, opt => opt.MapFrom(m => m.Category.Name));

            CreateMap<User, UserFullDTO>()
                .ForMember(m => m.Id, opt => opt.MapFrom(m => m.Id.ToString()))
                .ForMember(m => m.Role, opt => opt.MapFrom(m => m.Role.Name))
                .ForMember(m => m.Status, opt => opt.MapFrom(m => m.UnlockTime == null | m.UnlockTime < DateTime.Now ? null : $"Заблокирован до {m.UnlockTime}"))
                .ForMember(m => m.OrganizedEvents, opt => opt.MapFrom(m => m.OrganizedEvents.Count));
            CreateMap<User, UserLiteDTO>()
                .ForMember(m => m.Id, opt => opt.MapFrom(m => m.Id.ToString()))
                .ForMember(m => m.Role, opt => opt.MapFrom(m => m.Role.Name))
                .ForMember(m => m.Status, opt => opt.MapFrom(m => m.UnlockTime == null | m.UnlockTime < DateTime.Now ? null : $"Заблокирован до {m.UnlockTime}"));
            CreateMap<User, ChangeRoleDTO>()
                .ForMember(m => m.UserId, opt => opt.MapFrom(m => m.Id.ToString()));
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