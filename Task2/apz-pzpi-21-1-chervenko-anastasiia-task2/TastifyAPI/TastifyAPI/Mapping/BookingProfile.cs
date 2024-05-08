using AutoMapper;
using TastifyAPI.DTOs;
using TastifyAPI.DTOs.CreateDTOs;
using TastifyAPI.DTOs.UpdateDTOs;
using TastifyAPI.Entities;

namespace TastifyAPI.Mapping
{
    public class BookingProfile : Profile
    {
        public BookingProfile() 
        {
            CreateMap<Booking, BookingDto>();
            CreateMap<BookingDto, Booking>();
        }
        
    }
}
