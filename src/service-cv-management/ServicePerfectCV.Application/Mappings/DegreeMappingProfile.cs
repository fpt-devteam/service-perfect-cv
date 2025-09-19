using AutoMapper;
using ServicePerfectCV.Application.DTOs.Degree.Responses;
using ServicePerfectCV.Domain.Entities;

namespace ServicePerfectCV.Application.Mappings
{
    public class DegreeMappingProfile : Profile
    {
        public DegreeMappingProfile()
        {
            CreateMap<Degree, DegreeSuggestionResponse>();
        }
    }
}