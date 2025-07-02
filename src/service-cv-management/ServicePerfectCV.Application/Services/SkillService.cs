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
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly ICVSnapshotService _cvSnapshotService;

        public SkillService(
            ISkillRepository skillRepository,
            ICVRepository cvRepository,
            ICategoryRepository categoryRepository,
            IMapper mapper,
            ICVSnapshotService cvSnapshotService)
        {
            _skillRepository = skillRepository;
            _cvRepository = cvRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _cvSnapshotService = cvSnapshotService;
        }

        public async Task<SkillResponse> CreateAsync(Guid cvId, Guid userId, CreateSkillRequest request)
        {
            var cv = await _cvRepository.GetByCVIdAndUserIdAsync(cvId, userId) ??
                throw new DomainException(SkillErrors.CVNotFound);
            var category = await _categoryRepository.GetByNameAsync(request.CategoryName);
            if (category == null)
            {
                category = new Category
                {
                    Id = Guid.NewGuid(),
                    Name = request.CategoryName,
                    CreatedAt = DateTime.UtcNow
                };
                await _categoryRepository.CreateAsync(category);
                await _categoryRepository.SaveChangesAsync();
            }

            var newSkill = _mapper.Map<Skill>(request);
            newSkill.CVId = cvId;
            newSkill.CategoryId = category.Id;

            await _skillRepository.CreateAsync(newSkill);
            await _skillRepository.SaveChangesAsync();

            var skillWithCategory = await _skillRepository.GetByIdWithCategoryAsync(newSkill.Id);

            await _cvSnapshotService.UpdateCVSnapshotIfChangedAsync(cvId);

            return _mapper.Map<SkillResponse>(skillWithCategory);
        }

        public async Task<SkillResponse> UpdateAsync(Guid skillId, Guid cvId, Guid userId, UpdateSkillRequest request)
        {
            var skillExists = await _skillRepository.GetByIdAndCVIdAndUserIdAsync(skillId, cvId, userId)
                ?? throw new DomainException(SkillErrors.NotFound);

            var skillToUpdate = await _skillRepository.GetByIdAsync(skillId)
                ?? throw new DomainException(SkillErrors.NotFound);

            var category = await _categoryRepository.GetByNameAsync(request.CategoryName);
            if (category == null)
            {
                category = new Category
                {
                    Id = Guid.NewGuid(),
                    Name = request.CategoryName,
                    CreatedAt = DateTime.UtcNow
                };
                await _categoryRepository.CreateAsync(category);
                await _categoryRepository.SaveChangesAsync();
            }

            _mapper.Map(request, skillToUpdate);
            skillToUpdate.CategoryId = category.Id;
            skillToUpdate.UpdatedAt = DateTime.UtcNow;

            _skillRepository.Update(skillToUpdate);
            await _skillRepository.SaveChangesAsync();

            var updatedSkillWithCategory = await _skillRepository.GetByIdWithCategoryAsync(skillId);

            await _cvSnapshotService.UpdateCVSnapshotIfChangedAsync(cvId);

            return _mapper.Map<SkillResponse>(updatedSkillWithCategory);
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

            await _cvSnapshotService.UpdateCVSnapshotIfChangedAsync(cvId);
        }
    }
}
