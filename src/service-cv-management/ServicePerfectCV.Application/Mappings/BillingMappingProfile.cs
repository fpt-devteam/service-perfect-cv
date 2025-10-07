using AutoMapper;
using ServicePerfectCV.Application.DTOs.Billing.Responses;
using ServicePerfectCV.Domain.Entities;

namespace ServicePerfectCV.Application.Mappings
{
    public class BillingMappingProfile : Profile
    {
        public BillingMappingProfile()
        {
            CreateMap<BillingHistory, BillingHistoryResponse>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status));
        }
    }
}
