using AutoMapper;
using ServicePerfectCV.Application.DTOs.Certification.Requests;
using ServicePerfectCV.Application.DTOs.Certification.Responses;
using ServicePerfectCV.Domain.Entities;
using System;

namespace ServicePerfectCV.Application.Mappings
{
    public class CertificationMappingProfile : Profile
    {
        public CertificationMappingProfile()
        {
            CreateMap<Certification, CertificationResponse>();

            CreateMap<CreateCertificationRequest, Certification>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()));
        }
    }
}