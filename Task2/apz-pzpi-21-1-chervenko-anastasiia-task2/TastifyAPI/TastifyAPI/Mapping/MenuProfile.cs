using AutoMapper;
using TastifyAPI.DTOs;
using TastifyAPI.DTOs.CreateDTOs;
using TastifyAPI.DTOs.UpdateDTOs;
using TastifyAPI.Entities;

namespace TastifyAPI.Mapping
{
    public class MenuProfile : Profile
    {
        public MenuProfile() {
            CreateMap<Menu, MenuDto>();
            CreateMap<MenuCreateDto, Menu>();
            CreateMap<MenuUpdateDto, Menu>();
        }
        
    }
}
