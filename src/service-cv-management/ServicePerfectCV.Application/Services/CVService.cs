using AutoMapper;
using ServicePerfectCV.Application.DTOs.CV.Requests;
using ServicePerfectCV.Application.DTOs.CV.Responses;
using ServicePerfectCV.Application.DTOs.Pagination.Requests;
using ServicePerfectCV.Application.DTOs.Pagination.Responses;
using ServicePerfectCV.Application.Exceptions;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Services
{
    public class CVService(
        ICVRepository cvRepository,
        IJobDescriptionRepository jobDescriptionRepository,
        IMapper mapper
    )
    {
        public async Task<CVResponse> CreateAsync(CreateCVRequest request, Guid userId)
        {
            CV newCV = new CV
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = request.Title,
                CreatedAt = DateTime.UtcNow
            };

            await cvRepository.CreateAsync(newCV);
            await cvRepository.SaveChangesAsync();

            JobDescription jobDescription = new JobDescription
            {
                Id = Guid.NewGuid(),
                CVId = newCV.Id,
                Title = request.JobDescription.Title,
                CompanyName = request.JobDescription.CompanyName,
                Responsibility = request.JobDescription.Responsibility,
                Qualification = request.JobDescription.Qualification
            };

            await jobDescriptionRepository.CreateAsync(jobDescription);
            await jobDescriptionRepository.SaveChangesAsync();

            // Set the JobDescription navigation property for mapping
            newCV.JobDescription = jobDescription;
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

            cv.Title = request.Title ?? cv.Title;
            cv.UpdatedAt = DateTime.UtcNow;

            if (request.JobDescription != null)
            {
                var jobDescription = await jobDescriptionRepository.GetByCVIdAsync(cvId) ??
                    throw new DomainException(CVErrors.JobDescriptionNotFound);

                jobDescription.Title = request.JobDescription.Title ?? jobDescription.Title;
                jobDescription.CompanyName = request.JobDescription.CompanyName ?? jobDescription.CompanyName;
                jobDescription.Responsibility = request.JobDescription.Responsibility ?? jobDescription.Responsibility;
                jobDescription.Qualification = request.JobDescription.Qualification ?? jobDescription.Qualification;

                cv.JobDescription = jobDescription;
            }

            cvRepository.Update(cv);
            await cvRepository.SaveChangesAsync();

            return mapper.Map<CVResponse>(cv);
        }
        public async Task<CVResponse> GetByIdAndUserIdAsync(Guid cvId, Guid userId)
        {
            var cv = await cvRepository.GetByCVIdAndUserIdAsync(cvId, userId) ??
                throw new DomainException(CVErrors.CVNotFound);

            var jobDescription = await jobDescriptionRepository.GetByCVIdAsync(cvId) ??
                throw new DomainException(CVErrors.JobDescriptionNotFound);
            cv.JobDescription = jobDescription;

            return mapper.Map<CVResponse>(cv);

        }
        public async Task DeleteAsync(Guid cvId, Guid userId)
        {
            var deleted = await cvRepository.DeleteByCVIdAndUserIdAsync(cvId, userId);
            if (!deleted)
                throw new DomainException(CVErrors.CVNotFound);

            await jobDescriptionRepository.DeleteByCVIdAsync(cvId);
        }
    }
}