using ServicePerfectCV.Application.DTOs.CV;
using ServicePerfectCV.Application.DTOs.CV.Requests;
using ServicePerfectCV.Application.DTOs.CV.Responses;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Mappings
{
    public class CVMappingProfile : AuthMappingProfile
    {
        public CVMappingProfile()
        {
            CreateMap<JobDetail, JobDetailDto>();
            CreateMap<CV, CVSnapshotResponse>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.JobDetail, opt => opt.MapFrom(src => src.JobDetail))
                .ForMember(dest => dest.Educations, opt => opt.MapFrom(src => src.Educations));
            CreateMap<CreateCVRequest, CV>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.JobDetail, opt => opt.MapFrom(src => src.JobDetail))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
            CreateMap<CV, CVResponse>()
                .ForMember(dest => dest.CVId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.FullContent, opt => opt.MapFrom(src => src.FullContent))
                .ForMember(dest => dest.LastEditedAt, opt => opt.MapFrom(src => src.UpdatedAt ?? src.CreatedAt));
        }
    }
}