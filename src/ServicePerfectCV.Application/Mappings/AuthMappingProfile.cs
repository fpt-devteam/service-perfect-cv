using AutoMapper;
using ServicePerfectCV.Application.DTOs.Authentication.Requests;
using ServicePerfectCV.Application.DTOs.Authentication.Responses;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Mappings
{
    public class AuthMappingProfile : Profile
    {
        public AuthMappingProfile()
        {
            CreateMap<RegisterRequest, User>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<User, RegisterResponse>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src));

            CreateMap<LoginRequest, User>();


        }
    }
}