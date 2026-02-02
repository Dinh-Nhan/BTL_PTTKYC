using AutoMapper;
using backend.Dtos.Request;
using backend.Dtos.Response;
using backend.Models;

namespace backend.Mappings
{
    public class BookingProfile : Profile
    {

        public BookingProfile()
        {

            CreateMap<BookingRequest, Booking>()
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.PaymentStatus, opt => opt.Ignore())
                .ForMember(dest => dest.Client, opt => opt.Ignore())
                .ForMember(dest => dest.ClientId, opt => opt.Ignore())
                .ForSourceMember(src => src.Client, opt => opt.DoNotValidate());

            CreateMap<Booking, BookingResponse>()
                .ForMember(dest => dest.roomResponse, opt => opt.Ignore())
                .ForMember(dest => dest.clientResponse, opt => opt.Ignore());

        }
    }
}
