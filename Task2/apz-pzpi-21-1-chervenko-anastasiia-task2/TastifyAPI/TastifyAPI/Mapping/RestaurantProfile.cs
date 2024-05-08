using AutoMapper;
using TastifyAPI.DTOs;
using TastifyAPI.DTOs.CreateDTOs;
using TastifyAPI.DTOs.UpdateDTOs;
using TastifyAPI.Entities;

namespace TastifyAPI.Mapping
{
    public class RestaurantProfile : Profile
    {
        public RestaurantProfile()
        {
            CreateMap<Restaurant, RestaurantDto>();
            CreateMap<RestaurantDto, Restaurant>();
            CreateMap<RestaurantCreateDto, Restaurant>();
            CreateMap<RestaurantUpdateDto, Restaurant>();
        }
    }
}