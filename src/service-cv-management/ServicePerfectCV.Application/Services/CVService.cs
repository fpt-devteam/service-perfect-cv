using AutoMapper;
using ServicePerfectCV.Application.DTOs.CV.Requests;
using ServicePerfectCV.Application.DTOs.CV.Responses;
using ServicePerfectCV.Application.DTOs.Pagination.Requests;
using ServicePerfectCV.Application.DTOs.Pagination.Responses;
using ServicePerfectCV.Application.Exceptions;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Services
{
    public class CVService(
        ICVRepository cvRepository,
        IMapper mapper
    )
    {
        public async Task<CVResponse> CreateAsync(CreateCVRequest request, Guid userId)
        {
            CV newCV = mapper.Map<CV>(request);
            newCV.UserId = userId;

            await cvRepository.CreateAsync(newCV);
            await cvRepository.SaveChangesAsync();


            return mapper.Map<CVResponse>(newCV);
        }
        public async Task<PaginationData<CVResponse>> ListAsync(CVQuery query, Guid userId)
        {
            var cvs = await cvRepository.GetByUserIdAsync(
                query, userId);
            return new PaginationData<CVResponse>
            {
                Total = cvs.Count,
                Items = [.. cvs.Items.Select(cv => mapper.Map<CVResponse>(cv))]
            };
        }
        public async Task<CVResponse> UpdateAsync(Guid cvId, UpdateCVRequest request, Guid userId)
        {
            var cv = await cvRepository.GetByCVIdAndUserIdAsync(cvId, userId) ??
                throw new DomainException(CVErrors.CVNotFound);

            if (request.Title != null)
            {
                cv.Title = request.Title;
            }

            if (request.JobDetail != null)
            {
                cv.JobDetail = mapper.Map<JobDetail>(request.JobDetail);
            }

            if (request.AnalysisId.HasValue)
            {
                cv.AnalysisId = request.AnalysisId.Value;
            }

            cvRepository.Update(cv);
            await cvRepository.SaveChangesAsync();


            return mapper.Map<CVResponse>(cv);
        }

        public async Task<CVResponse> GetByIdAndUserIdAsync(Guid cvId, Guid userId)
        {
            var cv = await cvRepository.GetByCVIdAndUserIdAsync(cvId, userId) ??
                throw new DomainException(CVErrors.CVNotFound);

            return mapper.Map<CVResponse>(cv);

        }

        public async Task<CVFullContentResponse> GetFullContentAsync(Guid cvId, Guid userId)
        {
            var cv = await cvRepository.GetFullContentByCVIdAndUserIdAsync(cvId, userId) ??
                throw new DomainException(CVErrors.CVNotFound);

            return mapper.Map<CVFullContentResponse>(cv);
        }

        public async Task DeleteAsync(Guid cvId, Guid userId)
        {
            var deleted = await cvRepository.DeleteByCVIdAndUserIdAsync(cvId, userId);
            if (!deleted)
                throw new DomainException(CVErrors.CVNotFound);
        }
    }
}