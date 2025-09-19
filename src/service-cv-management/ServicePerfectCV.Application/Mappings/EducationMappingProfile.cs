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
            CreateMap<Education, EducationResponse>();
            CreateMap<CreateEducationRequest, Education>();
        }
    }
}