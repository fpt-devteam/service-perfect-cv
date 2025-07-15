using AutoMapper;
using ServicePerfectCV.Application.DTOs.Organization.Requests;
using ServicePerfectCV.Application.DTOs.Organization.Responses;
using ServicePerfectCV.Application.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Services
{
    public class OrganizationService(IOrganizationRepository organizationRepository, IMapper mapper)
    {
        public async Task<IEnumerable<OrganizationSuggestionResponse>> GetSuggestionsAsync(OrganizationQuery query)
        {
            var organizations = await organizationRepository.SearchByNameAsync(query);
            return mapper.Map<IEnumerable<OrganizationSuggestionResponse>>(organizations);
        }
    }
}
