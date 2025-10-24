using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServicePerfectCV.Application.Services;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Infrastructure.Data;
using ServicePerfectCV.WebAdmin.Models.Organizations;

namespace ServicePerfectCV.WebAdmin.Controllers
{
    [Authorize]
    public class OrganizationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly OrganizationService _organizationService;
        private readonly ILogger<OrganizationsController> _logger;

        public OrganizationsController(
            ApplicationDbContext context,
            OrganizationService organizationService,
            ILogger<OrganizationsController> logger)
        {
            _context = context;
            _organizationService = organizationService;
            _logger = logger;
        }

        // GET: Organizations
        public async Task<IActionResult> Index(string? search, int page = 1, int pageSize = 50)
        {
            ViewData["CurrentSearch"] = search;
            ViewData["CurrentPage"] = page;

            var query = _context.Organizations
                .Where(o => o.DeletedAt == null)
                .AsQueryable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(o =>
                    o.Name.ToLower().Contains(search) ||
                    (o.Description != null && o.Description.ToLower().Contains(search)));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var organizations = await query
                .OrderBy(o => o.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OrganizationListViewModel
                {
                    Id = o.Id,
                    Name = o.Name,
                    Description = o.Description,
                    LogoUrl = o.LogoUrl,
                    OrganizationType = o.OrganizationType,
                    CreatedAt = o.CreatedAt,
                    UpdatedAt = o.UpdatedAt,
                    UsageCount = 0 // Will be calculated later if needed
                })
                .ToListAsync();

            ViewBag.TotalPages = totalPages;
            ViewBag.TotalCount = totalCount;

            return View(organizations);
        }

        // GET: Organizations/Create
        public IActionResult Create()
        {
            return View(new EditOrganizationViewModel());
        }

        // POST: Organizations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EditOrganizationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Check for duplicates
                var exists = await _context.Organizations
                    .AnyAsync(o => o.Name.ToLower() == model.Name.ToLower() && o.DeletedAt == null);

                if (exists)
                {
                    ModelState.AddModelError("Name", "An organization with this name already exists.");
                    return View(model);
                }

                var organization = new Organization
                {
                    Id = Guid.NewGuid(),
                    Name = model.Name,
                    Description = model.Description,
                    LogoUrl = model.LogoUrl,
                    OrganizationType = model.OrganizationType,
                    CreatedAt = DateTimeOffset.UtcNow
                };

                _context.Organizations.Add(organization);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Organization {OrganizationId} created by admin {AdminEmail}",
                    organization.Id, User.Identity?.Name);
                TempData["SuccessMessage"] = "Organization created successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating organization");
                ModelState.AddModelError("", "Unable to save changes. Please try again.");
                return View(model);
            }
        }

        // GET: Organizations/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            var organization = await _context.Organizations
                .FirstOrDefaultAsync(o => o.Id == id && o.DeletedAt == null);

            if (organization == null)
            {
                TempData["ErrorMessage"] = "Organization not found.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new EditOrganizationViewModel
            {
                Id = organization.Id,
                Name = organization.Name,
                Description = organization.Description,
                LogoUrl = organization.LogoUrl,
                OrganizationType = organization.OrganizationType
            };

            return View(viewModel);
        }

        // POST: Organizations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, EditOrganizationViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var organization = await _context.Organizations
                    .FirstOrDefaultAsync(o => o.Id == id && o.DeletedAt == null);

                if (organization == null)
                {
                    TempData["ErrorMessage"] = "Organization not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Check for duplicates (excluding current)
                var duplicateExists = await _context.Organizations
                    .AnyAsync(o => o.Id != id &&
                        o.Name.ToLower() == model.Name.ToLower() &&
                        o.DeletedAt == null);

                if (duplicateExists)
                {
                    ModelState.AddModelError("Name", "An organization with this name already exists.");
                    return View(model);
                }

                organization.Name = model.Name;
                organization.Description = model.Description;
                organization.LogoUrl = model.LogoUrl;
                organization.OrganizationType = model.OrganizationType;
                organization.UpdatedAt = DateTimeOffset.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Organization {OrganizationId} updated by admin {AdminEmail}",
                    id, User.Identity?.Name);
                TempData["SuccessMessage"] = "Organization updated successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating organization {OrganizationId}", id);
                ModelState.AddModelError("", "Unable to save changes. Please try again.");
                return View(model);
            }
        }

        // POST: Organizations/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var organization = await _context.Organizations
                    .FirstOrDefaultAsync(o => o.Id == id && o.DeletedAt == null);

                if (organization == null)
                {
                    TempData["ErrorMessage"] = "Organization not found.";
                    return RedirectToAction(nameof(Index));
                }

                // For now, allow deletion without checking usage
                // In production, you might want to check if organization is referenced in educations/experiences/certifications

                // Soft delete
                organization.DeletedAt = DateTimeOffset.UtcNow;
                organization.UpdatedAt = DateTimeOffset.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Organization {OrganizationId} soft deleted by admin {AdminEmail}",
                    id, User.Identity?.Name);
                TempData["SuccessMessage"] = "Organization deleted successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting organization {OrganizationId}", id);
                TempData["ErrorMessage"] = "Unable to delete organization. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}

