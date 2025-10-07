using AutoMapper;
using ServicePerfectCV.Application.DTOs.EmploymentType.Requests;
using ServicePerfectCV.Application.DTOs.EmploymentType.Responses;
using ServicePerfectCV.Application.Interfaces.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Services
{
    public class EmploymentTypeService
    {
        private readonly IEmploymentTypeRepository _employmentTypeRepository;
        private readonly IMapper _mapper;

        public EmploymentTypeService(IEmploymentTypeRepository employmentTypeRepository, IMapper mapper)
        {
            _employmentTypeRepository = employmentTypeRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EmploymentTypeSuggestionResponse>> GetSuggestionsAsync(EmploymentTypeQuery query)
        {
            var employmentTypes = await _employmentTypeRepository.SearchByNameAsync(query);
            return _mapper.Map<IEnumerable<EmploymentTypeSuggestionResponse>>(employmentTypes);
        }
    }
}