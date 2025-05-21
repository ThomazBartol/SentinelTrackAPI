using AutoMapper;
using SentinelTrack.Application.DTOs.Request;
using SentinelTrack.Application.DTOs.Response;
using SentinelTrack.Domain.Entities;

namespace SentinelTrack.Application.Mappings
{
    public class MotoMapping : Profile
    {
        public MotoMapping()
        {
            CreateMap<Moto, MotoResponse>();
            CreateMap<MotoRequest, Moto>();
        }
    }
}
