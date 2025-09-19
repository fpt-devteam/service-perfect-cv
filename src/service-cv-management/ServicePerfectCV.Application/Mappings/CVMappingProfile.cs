using ServicePerfectCV.Application.DTOs.CV;
using ServicePerfectCV.Application.DTOs.CV.Requests;
using ServicePerfectCV.Application.DTOs.CV.Responses;
using ServicePerfectCV.Domain.Entities;
using System;

namespace ServicePerfectCV.Application.Mappings
{
    public class CVMappingProfile : AuthMappingProfile
    {
        public CVMappingProfile()
        {
            CreateMap<JobDescription, JobDescriptionResponse>();
            CreateMap<CreateJobDescriptionRequest, JobDescription>();
            CreateMap<UpdateJobDescriptionRequest, JobDescription>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<CreateCVRequest, CV>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.JobDescription, opt => opt.MapFrom(src => src.JobDescription))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
            CreateMap<CV, CVResponse>()
                .ForMember(dest => dest.CVId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.JobDescription, opt => opt.MapFrom(src => src.JobDescription))
                .ForMember(dest => dest.LastEditedAt, opt => opt.MapFrom(src => src.UpdatedAt ?? src.CreatedAt));
        }
    }
}