using AutoMapper;
using ServicePerfectCV.Application.DTOs.CV.Requests;
using ServicePerfectCV.Application.DTOs.Skill.Requests;
using ServicePerfectCV.Application.DTOs.Skill.Responses;
using ServicePerfectCV.Application.Exceptions;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Services
{
    public class SkillService
    {
        private readonly ISkillRepository _skillRepository;
        private readonly ICVRepository _cvRepository;
        private readonly IMapper _mapper;
        private readonly NotificationService _notificationService;

        public SkillService(
            ISkillRepository skillRepository,
            ICVRepository cvRepository,
            IMapper mapper,
            NotificationService notificationService)
        {
            _skillRepository = skillRepository;
            _cvRepository = cvRepository;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        public async Task<SkillResponse> CreateAsync(Guid cvId, Guid userId, CreateSkillRequest request)
        {
            var cv = await _cvRepository.GetByCVIdAndUserIdAsync(cvId, userId) ??
                throw new DomainException(SkillErrors.CVNotFound);

            var newSkill = _mapper.Map<Skill>(request);
            newSkill.CVId = cvId;

            await _skillRepository.CreateAsync(newSkill);
            await _skillRepository.SaveChangesAsync();


            // Send notification
            await _notificationService.SendSkillUpdateNotificationAsync(userId, "added");

            return _mapper.Map<SkillResponse>(newSkill);
        }

        public async Task<SkillResponse> UpdateAsync(Guid skillId, Guid cvId, Guid userId, UpdateSkillRequest request)
        {
            var skillToUpdate = await _skillRepository.GetByIdAndCVIdAndUserIdAsync(skillId, cvId, userId)
                ?? throw new DomainException(SkillErrors.NotFound);

            _mapper.Map(request, skillToUpdate);
            skillToUpdate.UpdatedAt = DateTime.UtcNow;

            _skillRepository.Update(skillToUpdate);
            await _skillRepository.SaveChangesAsync();


            // Send notification
            await _notificationService.SendSkillUpdateNotificationAsync(userId, "updated");

            return _mapper.Map<SkillResponse>(skillToUpdate);
        }

        public async Task<IEnumerable<SkillResponse>> GetByCVIdAsync(Guid cvId, Guid userId, SkillQuery query)
        {
            var skills = await _skillRepository.GetByCVIdAndUserIdAsync(cvId, userId, query);
            return _mapper.Map<IEnumerable<SkillResponse>>(skills);
        }

        public async Task<SkillResponse> GetByIdAsync(Guid skillId, Guid cvId, Guid userId)
        {
            var skill = await _skillRepository.GetByIdAndCVIdAndUserIdAsync(skillId, cvId, userId);
            if (skill == null)
                throw new DomainException(SkillErrors.NotFound);

            return _mapper.Map<SkillResponse>(skill);
        }

        public async Task DeleteAsync(Guid skillId, Guid cvId, Guid userId)
        {
            var skill = await _skillRepository.GetByIdAndCVIdAndUserIdAsync(skillId, cvId, userId);
            if (skill == null)
                throw new DomainException(SkillErrors.NotFound);

            skill.DeletedAt = DateTime.UtcNow;
            _skillRepository.Update(skill);
            await _skillRepository.SaveChangesAsync();


            // Send notification
            await _notificationService.SendSkillUpdateNotificationAsync(userId, "deleted");
        }
    }
}