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
        IMapper mapper
    )
    {
        public async Task<CVResponse> CreateCVAsync(CreateCVRequest request, Guid userId)
        {
            CV newCv = mapper.Map<CV>(request);
            newCv.UserId = userId;

            await cvRepository.CreateAsync(newCv);
            await cvRepository.SaveChangesAsync();
            return mapper.Map<CVResponse>(newCv);
        }
        public async Task<PaginationData<CVResponse>> ListAsync(PaginationRequest paginationRequest, Guid userId)
        {
            var cvs = await cvRepository.ListAsync(paginationRequest, userId);
            return new PaginationData<CVResponse>
            {
                Items = [.. cvs.Items.Select(cv => mapper.Map<CVResponse>(cv))],
                Total = cvs.Total
            };
        }

    }
}