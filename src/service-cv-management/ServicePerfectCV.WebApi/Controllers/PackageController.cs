using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePerfectCV.Application.DTOs.Package.Requests;
using ServicePerfectCV.Application.DTOs.Package.Responses;
using ServicePerfectCV.Application.Exceptions;
using ServicePerfectCV.Application.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ServicePerfectCV.WebApi.Controllers
{
    [ApiController]
    [Route("api/packages")]
    [Produces("application/json")]
    public class PackageController : ControllerBase
    {
        private readonly PackageService _packageService;

        public PackageController(PackageService packageService)
        {
            _packageService = packageService;
        }

        /// <summary>
        /// Get all active packages
        /// </summary>
        /// <returns>List of active packages</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<PackageResponse>), 200)]
        public async Task<ActionResult<IEnumerable<PackageResponse>>> GetActivePackages()
        {
            var packages = await _packageService.GetAllPackagesAsync();
            return Ok(packages);
        }

        /// <summary>
        /// Get all packages (admin only)
        /// </summary>
        /// <returns>List of all packages</returns>
        [HttpGet]
        [Authorize] // Add role-based authorization as needed
        [ProducesResponseType(typeof(IEnumerable<PackageResponse>), 200)]
        public async Task<ActionResult<IEnumerable<PackageResponse>>> GetAllPackages()
        {
            var packages = await _packageService.GetAllPackagesAsync();
            return Ok(packages);
        }

        /// <summary>
        /// Get package by ID
        /// </summary>
        /// <param name="id">Package ID</param>
        /// <returns>Package details</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(PackageResponse), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<PackageResponse>> GetPackageById([FromRoute] Guid id)
        {
            var package = await _packageService.GetPackageByIdAsync(id)
                ?? throw new DomainException(PackageErrors.NotFound);

            return Ok(package);
        }

        /// <summary>
        /// Get package by name
        /// </summary>
        /// <param name="name">Package name</param>
        /// <returns>Package details</returns>
        [HttpGet("by-name/{name}")]
        [ProducesResponseType(typeof(PackageResponse), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<PackageResponse>> GetPackageByName([FromRoute] string name)
        {
            var package = await _packageService.GetPackageByNameAsync(name)
                ?? throw new DomainException(PackageErrors.NotFound);

            return Ok(package);
        }

       

        /// <summary>
        /// Create a new package (admin only)
        /// </summary>
        /// <param name="request">Package creation request</param>
        /// <returns>Created package</returns>
        [HttpPost]
        [Authorize] // Add role-based authorization as needed
        [ProducesResponseType(typeof(PackageResponse), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        public async Task<ActionResult<PackageResponse>> CreatePackage([FromBody] CreatePackageRequest request)
        {
            var package = await _packageService.CreatePackageAsync(request);
            return CreatedAtAction(nameof(GetPackageById), new { id = package.Id }, package);
        }

        /// <summary>
        /// Update a package (admin only)
        /// </summary>
        /// <param name="id">Package ID</param>
        /// <param name="request">Package update request</param>
        /// <returns>Updated package</returns>
        [HttpPut("{id:guid}")]
        [Authorize] // Add role-based authorization as needed
        [ProducesResponseType(typeof(PackageResponse), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<ActionResult<PackageResponse>> UpdatePackage(
            [FromRoute] Guid id,
            [FromBody] UpdatePackageRequest request)
        {
            var package = await _packageService.UpdatePackageAsync(id, request);
            return Ok(package);
        }

        /// <summary>
        /// Delete a package (admin only)
        /// </summary>
        /// <param name="id">Package ID</param>
        /// <returns>Success confirmation</returns>
        [HttpDelete("{id:guid}")]
        [Authorize] // Add role-based authorization as needed
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<ActionResult> DeletePackage([FromRoute] Guid id)
        {
            await _packageService.DeletePackageAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Health check endpoint for package service
        /// </summary>
        /// <returns>Service health status</returns>
        [HttpGet("health")]
        [ProducesResponseType(200)]
        public ActionResult HealthCheck()
        {
            return Ok(new
            {
                Status = "Healthy",
                Service = "Package Service",
                Timestamp = DateTimeOffset.UtcNow
            });
        }
    }
}