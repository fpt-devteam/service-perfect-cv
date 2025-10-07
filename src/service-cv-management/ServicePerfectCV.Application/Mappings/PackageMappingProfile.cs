using AutoMapper;
using ServicePerfectCV.Application.DTOs.Package.Requests;
using ServicePerfectCV.Application.DTOs.Package.Responses;
using ServicePerfectCV.Domain.Entities;

namespace ServicePerfectCV.Application.Mappings
{
    public class PackageMappingProfile : Profile
    {
        public PackageMappingProfile()
        {
            // Entity to Response mapping
            CreateMap<Package, PackageResponse>()
                .ForMember(
                    dest => dest.TotalPurchases,
                    opt => opt.MapFrom(src => src.BillingHistories != null ? src.BillingHistories.Count : 0));

            // Request to Entity mappings
            CreateMap<CreatePackageRequest, Package>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.BillingHistories, opt => opt.Ignore());

            CreateMap<UpdatePackageRequest, Package>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.BillingHistories, opt => opt.Ignore());
        }
    }
}