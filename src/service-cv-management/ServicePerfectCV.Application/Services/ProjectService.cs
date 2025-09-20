using AutoMapper;
using ServicePerfectCV.Application.Common;
using ServicePerfectCV.Application.DTOs.Project.Requests;
using ServicePerfectCV.Application.DTOs.Project.Responses;
using ServicePerfectCV.Application.Exceptions;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Domain.Constants;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Services
{
    public class ProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ICVRepository _cvRepository;
        private readonly IMapper _mapper;

        public ProjectService(
            IProjectRepository projectRepository,
            ICVRepository cvRepository,
            IMapper mapper)
        {
            _projectRepository = projectRepository;
            _cvRepository = cvRepository;
            _mapper = mapper;
        }

        public async Task<ProjectResponse> CreateAsync(Guid cvId, Guid userId, CreateProjectRequest request)
        {
            var cv = await _cvRepository.GetByCVIdAndUserIdAsync(cvId, userId);
            if (cv == null)
                throw new DomainException(ProjectErrors.CVNotFound);

            var newProject = _mapper.Map<Project>(request);
            newProject.CVId = cvId;

            await _projectRepository.CreateAsync(newProject);
            await _projectRepository.SaveChangesAsync();

            return _mapper.Map<ProjectResponse>(newProject);
        }

        public async Task<ProjectResponse> UpdateAsync(Guid projectId, UpdateProjectRequest request)
        {
            var existingProject = await _projectRepository.GetByIdAsync(id: projectId)
                ?? throw new DomainException(ProjectErrors.NotFound);

            var cv = await _cvRepository.GetByIdAsync(existingProject.CVId);
            if (cv == null)
                throw new DomainException(ProjectErrors.CVNotFound);

            existingProject.Title = request.Title ?? existingProject.Title;
            existingProject.Description = request.Description ?? existingProject.Description;
            existingProject.Link = request.Link ?? existingProject.Link;
            existingProject.StartDate = request.StartDate ?? existingProject.StartDate;
            existingProject.EndDate = request.EndDate ?? existingProject.EndDate;
            existingProject.UpdatedAt = DateTimeOffset.UtcNow;

            _projectRepository.Update(existingProject);
            await _projectRepository.SaveChangesAsync();

            return _mapper.Map<ProjectResponse>(existingProject);
        }

        public async Task<ProjectResponse> GetByIdAsync(Guid projectId, Guid cvId, Guid userId)
        {
            var project = await _projectRepository.GetByIdAndCVIdAndUserIdAsync(id: projectId, cvId: cvId, userId: userId);
            if (project == null)
                throw new DomainException(ProjectErrors.NotFound);

            return _mapper.Map<ProjectResponse>(project);
        }

        public async Task<IEnumerable<ProjectResponse>> GetByCVIdAsync(Guid cvId, Guid userId, ProjectQuery query)
        {
            var projects = await _projectRepository.GetByCVIdAndUserIdAsync(cvId: cvId, userId: userId, query: query);
            return _mapper.Map<IEnumerable<ProjectResponse>>(projects);
        }

        public async Task DeleteAsync(Guid projectId, Guid cvId, Guid userId)
        {
            var project = await _projectRepository.GetByIdAndCVIdAndUserIdAsync(id: projectId, cvId: cvId, userId: userId);
            if (project == null)
                throw new DomainException(ProjectErrors.NotFound);

            project.DeletedAt = DateTimeOffset.UtcNow;
            _projectRepository.Update(project);
            await _projectRepository.SaveChangesAsync();
        }
    }
}