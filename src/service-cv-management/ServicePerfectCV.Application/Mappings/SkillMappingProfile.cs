using AutoMapper;
using ServicePerfectCV.Application.DTOs.Skill.Requests;
using ServicePerfectCV.Application.DTOs.Skill.Responses;
using ServicePerfectCV.Application.DTOs.Category.Responses;
using ServicePerfectCV.Domain.Entities;
using System;

namespace ServicePerfectCV.Application.Mappings
{
    public class SkillMappingProfile : Profile
    {
        public SkillMappingProfile()
        {
            CreateMap<Skill, SkillResponse>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category));
            
            CreateMap<CreateSkillRequest, Skill>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.CVId, opt => opt.Ignore()) 
                .ForMember(dest => dest.CategoryId, opt => opt.Ignore()) 
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            CreateMap<UpdateSkillRequest, Skill>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CVId, opt => opt.Ignore())
                .ForMember(dest => dest.CategoryId, opt => opt.Ignore()); 
            
            CreateMap<Category, CategoryResponse>();
        }
    }
}
