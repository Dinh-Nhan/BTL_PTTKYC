using AutoMapper;
using backend.Dtos.Request;
using backend.Dtos.Response;
using backend.Models;

namespace backend.Mappings
{
    public class RoomProfile : Profile
    {

        public RoomProfile()
        {
            // CreateMap<Source, Destination>();
            CreateMap<RoomType, RoomTypeResponse>();
            CreateMap<Room, RoomResponse>();
            CreateMap<updateRoomTypeRequest, RoomType>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<CreateRoomTypeRequest, RoomType>();
            CreateMap<Client, ClientResponse>();
            CreateMap<User, UserResponse>()
                .ForMember(
                    dest => dest.GenderValue,
                    opt => opt.MapFrom(src => (bool)src.Gender ? "Female" : "Male")
                )
                .ForMember(
                    dest => dest.RoleName,
                    opt => opt.MapFrom(src => (bool)src.RoleId ? "Employee" : "Admin")
                )
                .ForMember(
                    dest => dest.IsActiveStatus,
                    opt => opt.MapFrom(src => (bool)src.IsActive ? "Active" : "Inactive")
                );
            CreateMap<CreateEmployeeRequest, User>();
            CreateMap<UpdateEmployeeRequest, User>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<CreateEmployeeRequest, UserResponse>()
                .ForMember(
                    dest => dest.GenderValue,
                    opt => opt.MapFrom(src => (bool)src.Gender ? "Female" : "Male")
                )
                .ForMember(
                    dest => dest.RoleName,
                    opt => opt.MapFrom(src => (bool)src.RoleId ? "Employee" : "Admin")
                )
                .ForMember(
                    dest => dest.IsActiveStatus,
                    opt => opt.MapFrom(src => (bool)src.IsActive ? "Active" : "Inactive")
                ); 
            CreateMap<UpdateEmployeeRequest, UserResponse>()
                .ForMember(
                    dest => dest.GenderValue, 
                    opt => opt.MapFrom(src => (bool)src.Gender ? "Female" : "Male")
                )
                .ForMember(
                    dest => dest.IsActiveStatus,
                    opt => opt.MapFrom(src => (bool)src.IsActive ? "Active" : "Inactive")
                ); 
        }

    }
}
