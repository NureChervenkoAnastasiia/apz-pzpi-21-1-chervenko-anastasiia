using AutoMapper;
using TastifyAPI.DTOs;
using TastifyAPI.DTOs.CreateDTOs;
using TastifyAPI.DTOs.UpdateDTOs;
using TastifyAPI.Entities;

namespace TastifyAPI.Mapping
{
    public class StaffProfile : Profile
    {
        public StaffProfile()
        {
            CreateMap<Staff, StaffDto>();
            CreateMap<StaffCreateDto, Staff>();
            CreateMap<StaffUpdateDTO, Staff>();
        }
    }
}