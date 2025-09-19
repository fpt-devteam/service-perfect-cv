using AutoMapper;
using ServicePerfectCV.Application.DTOs.Category.Responses;
using ServicePerfectCV.Domain.Entities;

namespace ServicePerfectCV.Application.Mappings
{
    public class CategoryMappingProfile : Profile
    {
        public CategoryMappingProfile()
        {
            CreateMap<Category, CategorySuggestionResponse>();
            CreateMap<Category, CategoryResponse>();
        }
    }
}