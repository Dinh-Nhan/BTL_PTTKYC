using AutoMapper;
using backend.Dtos.Response;
using backend.Models;

namespace backend.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserResponse>()
                .ForMember(
                dest => dest.Gender,
                opt => opt.MapFrom(src => src.Gender.Value ? "Nam" : "Nữ")
            );
        }
    }
}
