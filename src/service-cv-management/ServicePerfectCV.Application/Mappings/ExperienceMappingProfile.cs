using AutoMapper;
using ServicePerfectCV.Application.DTOs.Experience.Requests;
using ServicePerfectCV.Application.DTOs.Experience.Responses;
using ServicePerfectCV.Domain.Entities;

namespace ServicePerfectCV.Application.Mappings
{
    public class ExperienceMappingProfile : Profile
    {
        public ExperienceMappingProfile()
        {
            CreateMap<Experience, ExperienceResponse>()
                .ForMember(
                    dest => dest.EmploymentTypeName,
                    opt => opt.MapFrom(src => src.EmploymentType.Name));

            CreateMap<CreateExperienceRequest, Experience>();
        }
    }
}