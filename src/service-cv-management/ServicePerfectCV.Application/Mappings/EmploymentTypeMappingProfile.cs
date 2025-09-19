using AutoMapper;
using ServicePerfectCV.Application.DTOs.EmploymentType.Responses;
using ServicePerfectCV.Domain.Entities;

namespace ServicePerfectCV.Application.Mappings
{
    public class EmploymentTypeMappingProfile : Profile
    {
        public EmploymentTypeMappingProfile()
        {
            CreateMap<EmploymentType, EmploymentTypeSuggestionResponse>();
        }
    }
}