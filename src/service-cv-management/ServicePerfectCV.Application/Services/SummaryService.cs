using AutoMapper;
using ServicePerfectCV.Application.Common;
using ServicePerfectCV.Application.DTOs.Summary.Requests;
using ServicePerfectCV.Application.DTOs.Summary.Responses;
using ServicePerfectCV.Application.Exceptions;
using ServicePerfectCV.Application.Interfaces.Repositories;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Services
{
    public class SummaryService
    {
        private readonly ISummaryRepository _summaryRepository;
        private readonly ICVRepository _cvRepository;
        private readonly IMapper _mapper;
        private readonly NotificationService _notificationService;

        public SummaryService(
            ISummaryRepository summaryRepository,
            ICVRepository cvRepository,
            IMapper mapper,
            NotificationService notificationService)
        {
            _summaryRepository = summaryRepository;
            _cvRepository = cvRepository;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        public async Task<SummaryResponse> UpsertAsync(Guid cvId, Guid userId, UpsertSummaryRequest request)
        {
            var cv = await _cvRepository.GetByCVIdAndUserIdAsync(cvId, userId) ?? throw new DomainException(SummaryErrors.CVNotFound);

            var existingSummary = await _summaryRepository.GetByCVIdAsync(cvId);

            if (existingSummary == null)
            {
                var newSummary = _mapper.Map<Summary>(request);
                newSummary.CVId = cvId;

                await _summaryRepository.CreateAsync(newSummary);
                await _summaryRepository.SaveChangesAsync();

                return _mapper.Map<SummaryResponse>(newSummary);
            }

            existingSummary.UpdateIfPresent(s => s.Content, request.SummaryContent);

            _summaryRepository.Update(existingSummary);
            await _summaryRepository.SaveChangesAsync();

            return _mapper.Map<SummaryResponse>(existingSummary);
        }

        public async Task<SummaryResponse> GetByCVIdAsync(Guid cvId)
        {
            var summary = await _summaryRepository.GetByCVIdAsync(cvId) ?? throw new DomainException(SummaryErrors.NotFound);
            return _mapper.Map<SummaryResponse>(summary);
        }
    }
}