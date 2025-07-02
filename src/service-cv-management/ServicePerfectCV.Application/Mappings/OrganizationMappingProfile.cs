using AutoMapper;
using ServicePerfectCV.Application.DTOs.Organization.Responses;
using ServicePerfectCV.Domain.Entities;

namespace ServicePerfectCV.Application.Mappings
{
    public class OrganizationMappingProfile : Profile
    {
        public OrganizationMappingProfile()
        {
            CreateMap<Organization, OrganizationSuggestionResponse>()
                .ForMember(dest => dest.OrganizationType, opt => opt.MapFrom(src => src.OrganizationType.ToString()));
        }
    }
}
