using AutoMapper;
using ServicePerfectCV.Application.DTOs.Education.Requests;
using ServicePerfectCV.Application.DTOs.Education.Responses;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Services
{
    public class EducationService(
        IEducationRepository educationRepository,
        IMapper mapper)
    {
        public async Task<EducationResponse> CreateAsync(CreateEducationRequest request)
        {
            var newEducation = mapper.Map<Education>(request);

            await educationRepository.CreateAsync(newEducation);
            await educationRepository.SaveChangesAsync();
            return mapper.Map<EducationResponse>(newEducation);
        }
        
        public async Task<IEnumerable<EducationResponse>> ListAsync(Guid cvId)
        {
            var educations = await educationRepository.ListAsync(cvId);
            return educations.Select(e => mapper.Map<EducationResponse>(e));
        }
    }
}