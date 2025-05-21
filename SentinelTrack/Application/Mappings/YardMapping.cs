using AutoMapper;
using SentinelTrack.Application.DTOs.Request;
using SentinelTrack.Application.DTOs.Response;
using SentinelTrack.Domain.Entities;

namespace SentinelTrack.Application.Mappings
{
    public class YardMapping : Profile
    {
        public YardMapping() 
        {
            CreateMap<YardRequest, Yard>();
            CreateMap<Yard, YardResponse>()
            .ForMember(dest => dest.Motos, opt => opt.MapFrom(src => src.Motos));
            CreateMap<Moto, MotoResponse>();
        }
    }
}
