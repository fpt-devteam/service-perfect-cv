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
            CreateMap<JobDescription, JobDescriptionDto>();
            CreateMap<JobDescriptionDto, JobDescription>();
            CreateMap<CreateCVRequest, CV>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.JobDescription, opt => opt.MapFrom(src => src.JobDescription))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
            CreateMap<CV, CVResponse>()
                .ForMember(dest => dest.CVId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.FullContent, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.LastEditedAt, opt => opt.MapFrom(src => src.UpdatedAt ?? src.CreatedAt));
            CreateMap<CV, CVFullContentResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.VersionId, opt => opt.MapFrom(src => src.VersionId))
                .ForMember(dest => dest.AnalysisId, opt => opt.MapFrom(src => src.AnalysisId))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ForMember(dest => dest.JobDescription, opt => opt.MapFrom(src => src.JobDescription))
                .ForMember(dest => dest.Contact, opt => opt.MapFrom(src => src.Contact))
                .ForMember(dest => dest.Summary, opt => opt.MapFrom(src => src.Summary))
                .ForMember(dest => dest.Skills, opt => opt.MapFrom(src => src.Skills))
                .ForMember(dest => dest.Educations, opt => opt.MapFrom(src => src.Educations))
                .ForMember(dest => dest.Experiences, opt => opt.MapFrom(src => src.Experiences))
                .ForMember(dest => dest.Projects, opt => opt.MapFrom(src => src.Projects))
                .ForMember(dest => dest.Certifications, opt => opt.MapFrom(src => src.Certifications));
        }
    }
}