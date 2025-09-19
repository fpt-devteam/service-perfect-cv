using AutoMapper;
using ServicePerfectCV.Application.DTOs.JobTitle.Responses;
using ServicePerfectCV.Domain.Entities;

namespace ServicePerfectCV.Application.Mappings
{
    public class JobTitleMappingProfile : Profile
    {
        public JobTitleMappingProfile()
        {
            CreateMap<JobTitle, JobTitleSuggestionResponse>();
        }
    }
}