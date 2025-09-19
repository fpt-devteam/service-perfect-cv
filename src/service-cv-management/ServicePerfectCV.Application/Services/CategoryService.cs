using AutoMapper;
using ServicePerfectCV.Application.DTOs.Category.Requests;
using ServicePerfectCV.Application.DTOs.Category.Responses;
using ServicePerfectCV.Application.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Services
{
    public class CategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategorySuggestionResponse>> GetSuggestionsAsync(CategoryQuery query)
        {
            var categories = await _categoryRepository.SearchByNameAsync(query);
            return _mapper.Map<IEnumerable<CategorySuggestionResponse>>(categories);
        }
    }
}