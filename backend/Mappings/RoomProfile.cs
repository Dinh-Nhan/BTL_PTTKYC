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
        }

    }
}
