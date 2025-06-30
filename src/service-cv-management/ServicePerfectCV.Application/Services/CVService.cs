using AutoMapper;
using ServicePerfectCV.Application.DTOs.CV.Requests;
using ServicePerfectCV.Application.DTOs.CV.Responses;
using ServicePerfectCV.Application.DTOs.Pagination.Requests;
using ServicePerfectCV.Application.DTOs.Pagination.Responses;
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
        IMapper mapper,
        ICVSnapshotService cvSnapshotService
    )
    {
        public async Task<CVResponse> CreateAsync(CreateCVRequest request, Guid userId)
        {
            CV newCV = mapper.Map<CV>(request);
            newCV.UserId = userId;

            await cvRepository.CreateAsync(newCV);
            await cvRepository.SaveChangesAsync();

            await cvSnapshotService.UpdateCVSnapshotIfChangedAsync(newCV.Id);

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

    }
}