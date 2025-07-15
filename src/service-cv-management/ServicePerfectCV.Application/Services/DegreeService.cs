using AutoMapper;
using ServicePerfectCV.Application.DTOs.Degree.Requests;
using ServicePerfectCV.Application.DTOs.Degree.Responses;
using ServicePerfectCV.Application.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Services
{
    public class DegreeService
    {
        private readonly IDegreeRepository _degreeRepository;
        private readonly IMapper _mapper;

        public DegreeService(IDegreeRepository degreeRepository, IMapper mapper)
        {
            _degreeRepository = degreeRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DegreeSuggestionResponse>> GetSuggestionsAsync(DegreeQuery query)
        {
            var degrees = await _degreeRepository.SearchByNameAsync(query);
            return _mapper.Map<IEnumerable<DegreeSuggestionResponse>>(degrees);
        }
    }
}
