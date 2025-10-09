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
    }
}