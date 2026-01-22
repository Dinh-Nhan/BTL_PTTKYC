using AutoMapper;
using backend.Dtos.Request;
using backend.Dtos.Response;
using backend.Models;

namespace backend.Mappings
{
    public class ClientProfile : Profile
    {

        public ClientProfile()
        {
            CreateMap<ClientRequest, Client>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
            CreateMap<Client, ClientResponse>();
        }
    }
}
