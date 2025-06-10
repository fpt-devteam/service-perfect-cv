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
                .ForMember(dest => dest.Institution, opt => opt.MapFrom(src => src.Institution))
                .ForMember(dest => dest.Degree, opt => opt.MapFrom(src => src.Degree))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
                .ForMember(dest => dest.YearObtained, opt => opt.MapFrom(src => src.YearObtained))
                .ForMember(dest => dest.Minor, opt => opt.MapFrom(src => src.Minor))
                .ForMember(dest => dest.Gpa, opt => opt.MapFrom(src => src.Gpa))
                .ForMember(dest => dest.AdditionalInfo, opt => opt.MapFrom(src => src.AdditionalInfo));

            CreateMap<CreateEducationRequest, Education>()
                .ForMember(dest => dest.CVId, opt => opt.MapFrom(src => src.CVId))
                .ForMember(dest => dest.Institution, opt => opt.MapFrom(src => src.Institution))
                .ForMember(dest => dest.Degree, opt => opt.MapFrom(src => src.Degree))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
                .ForMember(dest => dest.YearObtained, opt => opt.MapFrom(src => src.YearObtained))
                .ForMember(dest => dest.Minor, opt => opt.MapFrom(src => src.Minor))
                .ForMember(dest => dest.Gpa, opt => opt.MapFrom(src => src.Gpa))
                .ForMember(dest => dest.AdditionalInfo, opt => opt.MapFrom(src => src.AdditionalInfo));
        }
    }
}