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

            CreateMap<EventCreateDTO, Event>()
                .ForMember(m => m.HasPhoto, opt => opt.MapFrom(m => m.Image != null))
                .ForMember(m => m.PublicationTime, opt => opt.MapFrom(m => DateTime.Now));
            CreateMap<string, Tag>()
                .ForMember(m => m.Name, opt => opt.MapFrom(m => m));
            CreateMap<EventUpdateDTO, Event>();
            CreateMap<Event, EventFullDTO>()
                .ForMember(m => m.Category, opt => opt.MapFrom(m => m.Category.Name))
                .ForMember(m => m.Organizer, opt => opt.MapFrom(m => m.Organizer.UserName));
            CreateMap<Event, EventLiteDTO>()
                .ForMember(m => m.Category, opt => opt.MapFrom(m => m.Category.Name));

            CreateMap<User, UserFullDTO>()
                .ForMember(m => m.Role, opt => opt.MapFrom(m => m.Role.Name))
                .ForMember(m => m.Status, opt => opt.MapFrom(m => m.UnlockTime == null | m.UnlockTime < DateTime.Now ? null : $"Заблокирован до {m.UnlockTime}"))
                .ForMember(m => m.OrganizedEvents, opt => opt.MapFrom(m => m.OrganizedEvents.Count));
            CreateMap<User, UserLiteDTO>()
                .ForMember(m => m.Role, opt => opt.MapFrom(m => m.Role.Name))
                .ForMember(m => m.Status, opt => opt.MapFrom(m => m.UnlockTime == null | m.UnlockTime < DateTime.Now ? null : $"Заблокирован до {m.UnlockTime}"));
            CreateMap<UserUpdateDTO, User>();
            CreateMap<RegisterDTO, User>()
                .ForMember(m => m.Password, opt => opt.MapFrom(m => HashGenerator.Encrypt(m.Password)))
                .ForMember(m => m.RegistrationDate, opt => opt.MapFrom(m => DateTime.Now));
        }
    }
}