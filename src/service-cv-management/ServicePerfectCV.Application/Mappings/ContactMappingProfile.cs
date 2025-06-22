using AutoMapper;
using ServicePerfectCV.Application.DTOs.Contact.Requests;
using ServicePerfectCV.Application.DTOs.Contact.Responses;
using ServicePerfectCV.Domain.Entities;
using System;

namespace ServicePerfectCV.Application.Mappings
{
    public class ContactMappingProfile : Profile
    {
        public ContactMappingProfile()
        {
            CreateMap<Contact, ContactResponse>();
            
            CreateMap<UpsertContactRequest, Contact>()
                .ForMember(dest => dest.CVId, opt => opt.MapFrom(src => src.CVId))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.LinkedInUrl, opt => opt.MapFrom(src => src.LinkedInUrl))
                .ForMember(dest => dest.GitHubUrl, opt => opt.MapFrom(src => src.GitHubUrl))
                .ForMember(dest => dest.PersonalWebsiteUrl, opt => opt.MapFrom(src => src.PersonalWebsiteUrl))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City));
        }
    }
}
