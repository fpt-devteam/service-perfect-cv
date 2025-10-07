using AutoMapper;
using ServicePerfectCV.Application.DTOs.JobTitle.Requests;
using ServicePerfectCV.Application.DTOs.JobTitle.Responses;
using ServicePerfectCV.Application.Interfaces.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Services
{
    public class JobTitleService
    {
        private readonly IJobTitleRepository _jobTitleRepository;
        private readonly IMapper _mapper;

        public JobTitleService(IJobTitleRepository jobTitleRepository, IMapper mapper)
        {
            _jobTitleRepository = jobTitleRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<JobTitleSuggestionResponse>> GetSuggestionsAsync(JobTitleQuery query)
        {
            var jobTitles = await _jobTitleRepository.SearchByNameAsync(query);
            return _mapper.Map<IEnumerable<JobTitleSuggestionResponse>>(jobTitles);
        }
    }
}