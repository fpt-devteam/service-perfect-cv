using AutoMapper;
using ServicePerfectCV.Application.DTOs.Package.Requests;
using ServicePerfectCV.Application.DTOs.Package.Responses;
using ServicePerfectCV.Application.Exceptions;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Services
{
    public class PackageService
    {
        private readonly IPackageRepository _packageRepository;
        private readonly IMapper _mapper;

        public PackageService(IPackageRepository packageRepository, IMapper mapper)
        {
            _packageRepository = packageRepository;
            _mapper = mapper;
        }

        public async Task<PackageResponse> CreatePackageAsync(CreatePackageRequest request)
        {
            // Check if package name already exists
            var existingPackage = await _packageRepository.GetByNameAsync(request.Name);
            if (existingPackage != null)
            {
                throw new DomainException(PackageErrors.NameAlreadyExists);
            }

            var package = _mapper.Map<Package>(request);
            package.Id = Guid.NewGuid();
            package.CreatedAt = DateTimeOffset.UtcNow;

            await _packageRepository.CreateAsync(package);
            await _packageRepository.SaveChangesAsync();

            return _mapper.Map<PackageResponse>(package);
        }

        public async Task<PackageResponse> UpdatePackageAsync(Guid id, UpdatePackageRequest request)
        {
            var package = await _packageRepository.GetByIdAsync(id)
                ?? throw new DomainException(PackageErrors.NotFound);

            // Check if new name conflicts with existing package (excluding current)
            var existingPackage = await _packageRepository.GetByNameAsync(request.Name);
            if (existingPackage != null && existingPackage.Id != id)
            {
                throw new DomainException(PackageErrors.NameAlreadyExists);
            }

            _mapper.Map(request, package);
            package.UpdatedAt = DateTimeOffset.UtcNow;

            _packageRepository.Update(package);
            await _packageRepository.SaveChangesAsync();

            return _mapper.Map<PackageResponse>(package);
        }

        public async Task<bool> DeletePackageAsync(Guid id)
        {
            var package = await _packageRepository.GetByIdAsync(id)
                ?? throw new DomainException(PackageErrors.NotFound);

            // Check if package has any purchases
            if (package.BillingHistories.Any())
            {
                throw new DomainException(PackageErrors.CannotDeletePackageWithPurchases);
            }

            package.DeletedAt = DateTimeOffset.UtcNow;
            _packageRepository.Update(package);
            await _packageRepository.SaveChangesAsync();

            return true;
        }

        public async Task<PackageResponse?> GetPackageByIdAsync(Guid id)
        {
            var package = await _packageRepository.GetByIdAsync(id);
            return package != null ? _mapper.Map<PackageResponse>(package) : null;
        }

        public async Task<IEnumerable<PackageResponse>> GetAllPackagesAsync()
        {
            var packages = await _packageRepository.GetAllPackagesAsync();
            return _mapper.Map<IEnumerable<PackageResponse>>(packages);
        }

        public async Task<PackageResponse?> GetPackageByNameAsync(string name)
        {
            var package = await _packageRepository.GetByNameAsync(name);
            return package != null ? _mapper.Map<PackageResponse>(package) : null;
        }

    }
}