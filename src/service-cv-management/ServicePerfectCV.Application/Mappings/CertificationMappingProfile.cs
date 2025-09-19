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
            CreateMap<Certification, CertificationResponse>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Organization, opt => opt.MapFrom(src => src.Organization))
                .ForMember(dest => dest.IssuedDate, opt => opt.MapFrom(src => src.IssuedDate))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

            CreateMap<CreateCertificationRequest, Certification>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Organization, opt => opt.MapFrom(src => src.Organization))
                .ForMember(dest => dest.IssuedDate, opt => opt.MapFrom(src => src.IssuedDate))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));
        }
    }
}