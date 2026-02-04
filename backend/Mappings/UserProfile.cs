using AutoMapper;
using backend.Dtos.Request;
using backend.Dtos.Response;
using backend.Models;

namespace backend.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserResponse>()
            .ForMember(dest => dest.GenderValue, opt => opt.Ignore())
            .ForMember(
                dest => dest.GenderValue,
                opt => opt.MapFrom(src => src.Gender == true ? "Nữ" : "Nam")
            )
            .ForMember(
                dest => dest.RoleName,
                opt => opt.MapFrom(src => src.RoleId == true ? "Nhân viên" : "Quản trị viên")
            )
            .ForMember(
                dest => dest.IsActiveStatus,
                opt => opt.MapFrom(src => src.IsActive == true ? "Đang hoạt động" : "Ngừng hoạt động")
            );
            CreateMap<CreateEmployeeRequest, UserResponse>()
                .ForMember(
                    dest => dest.GenderValue,
                    opt => opt.MapFrom(src => (bool)src.Gender ? "Nữ" : "Nam")
                )
                .ForMember(
                    dest => dest.RoleName,
                    opt => opt.MapFrom(src => (bool)src.RoleId ? "Nhân viên" : "Quản trị viên")
                )
                .ForMember(
                    dest => dest.IsActiveStatus,
                    opt => opt.MapFrom(src => (bool)src.IsActive ? "Đang hoạt động" : "Ngừng hoạt động")
                );
            CreateMap<UpdateEmployeeRequest, UserResponse>()
                .ForMember(
                    dest => dest.GenderValue,
                    opt => opt.MapFrom(src => (bool)src.Gender ? "Nữ" : "Nam")
                )
                .ForMember(
                    dest => dest.IsActiveStatus,
                    opt => opt.MapFrom(src => (bool)src.IsActive ? "Đang hoạt động" : "Ngừng hoạt động")
                );
            CreateMap<CreateEmployeeRequest, User>();
            CreateMap<UpdateEmployeeRequest, User>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        }
    }
}
