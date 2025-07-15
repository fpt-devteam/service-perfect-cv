using ServicePerfectCV.Application.DTOs.Education.Requests;
using ServicePerfectCV.Application.DTOs.Education.Responses;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Mappings
{
    public class EducationMappingProfile : AuthMappingProfile
    {
        public EducationMappingProfile()
        {
            CreateMap<Education, EducationResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Organization, opt => opt.MapFrom(src => src.Organization))
                .ForMember(dest => dest.Degree, opt => opt.MapFrom(src => src.Degree))
                .ForMember(dest => dest.FieldOfStudy, opt => opt.MapFrom(src => src.FieldOfStudy))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Gpa, opt => opt.MapFrom(src => src.Gpa));

            CreateMap<CreateEducationRequest, Education>()
                .ForMember(dest => dest.Degree, opt => opt.MapFrom(src => src.Degree))
                .ForMember(dest => dest.Organization, opt => opt.MapFrom(src => src.Organization))
                .ForMember(dest => dest.FieldOfStudy, opt => opt.MapFrom(src => src.FieldOfStudy))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Gpa, opt => opt.MapFrom(src => src.Gpa));
        }
    }
}