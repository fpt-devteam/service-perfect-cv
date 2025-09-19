using AutoMapper;
using ServicePerfectCV.Application.DTOs.Summary.Requests;
using ServicePerfectCV.Application.DTOs.Summary.Responses;
using ServicePerfectCV.Domain.Entities;
using System;

namespace ServicePerfectCV.Application.Mappings
{
    public class SummaryMappingProfile : Profile
    {
        public SummaryMappingProfile()
        {
            CreateMap<Summary, SummaryResponse>();

            CreateMap<UpsertSummaryRequest, Summary>()
                .ForMember(dest => dest.CVId, opt => opt.MapFrom(src => src.CVId))
                .ForMember(dest => dest.Context, opt => opt.MapFrom(src => src.Context))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()));
        }
    }
}