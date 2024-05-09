using AutoMapper;
using TastifyAPI.DTOs;
using TastifyAPI.Entities;

namespace TastifyAPI.Mapping
{
    public class PositionProductProfile : Profile
    {
        public PositionProductProfile()
        {
            CreateMap<PositionProduct, PositionProductDto>();
            CreateMap<PositionProductDto, PositionProduct>();
        }
    }

}
