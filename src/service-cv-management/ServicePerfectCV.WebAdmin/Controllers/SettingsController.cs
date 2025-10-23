using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Infrastructure.Data;
using ServicePerfectCV.WebAdmin.Models.Settings;

namespace ServicePerfectCV.WebAdmin.Controllers
{
  [Authorize]
  public class SettingsController : Controller
  {
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SettingsController> _logger;

    public SettingsController(
        ApplicationDbContext context,
        ILogger<SettingsController> logger)
    {
      _context = context;
      _logger = logger;
    }

    // GET: Settings
    public IActionResult Index()
    {
      return View();
    }

    #region Degrees

    // GET: Settings/Degrees
    public async Task<IActionResult> Degrees()
    {
      var degrees = await _context.Degrees
          .Where(d => d.DeletedAt == null)
          .OrderBy(d => d.Name)
          .Select(d => new DegreeViewModel
          {
            Id = d.Id,
            Code = d.Code,
            Name = d.Name,
            CreatedAt = d.CreatedAt,
            UsageCount = 0 // Simplified - not tracking usage
          })
          .ToListAsync();

      return View(degrees);
    }

    // POST: Settings/CreateDegree
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateDegree(DegreeViewModel model)
    {
      if (!ModelState.IsValid)
      {
        TempData["ErrorMessage"] = "Invalid data provided.";
        return RedirectToAction(nameof(Degrees));
      }

      try
      {
        // Check for duplicates
        var exists = await _context.Degrees
            .AnyAsync(d => (d.Code.ToLower() == model.Code.ToLower() ||
                           d.Name.ToLower() == model.Name.ToLower()) &&
                           d.DeletedAt == null);

        if (exists)
        {
          TempData["ErrorMessage"] = "A degree with this code or name already exists.";
          return RedirectToAction(nameof(Degrees));
        }

        var degree = new Degree
        {
          Id = Guid.NewGuid(),
          Code = model.Code,
          Name = model.Name,
          CreatedAt = DateTimeOffset.UtcNow
        };

        _context.Degrees.Add(degree);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Degree {DegreeId} created by admin {AdminEmail}",
            degree.Id, User.Identity?.Name);
        TempData["SuccessMessage"] = "Degree created successfully.";

        return RedirectToAction(nameof(Degrees));
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error creating degree");
        TempData["ErrorMessage"] = "Unable to create degree. Please try again.";
        return RedirectToAction(nameof(Degrees));
      }
    }

    // POST: Settings/UpdateDegree
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateDegree(Guid id, DegreeViewModel model)
    {
      if (!ModelState.IsValid)
      {
        TempData["ErrorMessage"] = "Invalid data provided.";
        return RedirectToAction(nameof(Degrees));
      }

      try
      {
        var degree = await _context.Degrees
            .FirstOrDefaultAsync(d => d.Id == id && d.DeletedAt == null);

        if (degree == null)
        {
          TempData["ErrorMessage"] = "Degree not found.";
          return RedirectToAction(nameof(Degrees));
        }

        // Check for duplicates (excluding current)
        var duplicateExists = await _context.Degrees
            .AnyAsync(d => d.Id != id &&
                (d.Code.ToLower() == model.Code.ToLower() ||
                 d.Name.ToLower() == model.Name.ToLower()) &&
                d.DeletedAt == null);

        if (duplicateExists)
        {
          TempData["ErrorMessage"] = "A degree with this code or name already exists.";
          return RedirectToAction(nameof(Degrees));
        }

        degree.Code = model.Code;
        degree.Name = model.Name;
        degree.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Degree {DegreeId} updated by admin {AdminEmail}",
            id, User.Identity?.Name);
        TempData["SuccessMessage"] = "Degree updated successfully.";

        return RedirectToAction(nameof(Degrees));
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error updating degree {DegreeId}", id);
        TempData["ErrorMessage"] = "Unable to update degree. Please try again.";
        return RedirectToAction(nameof(Degrees));
      }
    }

    // POST: Settings/DeleteDegree
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteDegree(Guid id)
    {
      try
      {
        var degree = await _context.Degrees
            .FirstOrDefaultAsync(d => d.Id == id && d.DeletedAt == null);

        if (degree == null)
        {
          TempData["ErrorMessage"] = "Degree not found.";
          return RedirectToAction(nameof(Degrees));
        }

        // Soft delete
        degree.DeletedAt = DateTimeOffset.UtcNow;
        degree.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Degree {DegreeId} soft deleted by admin {AdminEmail}",
            id, User.Identity?.Name);
        TempData["SuccessMessage"] = "Degree deleted successfully.";

        return RedirectToAction(nameof(Degrees));
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error deleting degree {DegreeId}", id);
        TempData["ErrorMessage"] = "Unable to delete degree. Please try again.";
        return RedirectToAction(nameof(Degrees));
      }
    }

    #endregion

    #region Job Titles

    // GET: Settings/JobTitles
    public async Task<IActionResult> JobTitles()
    {
      var jobTitles = await _context.JobTitles
          .Where(jt => jt.DeletedAt == null)
          .OrderBy(jt => jt.Name)
          .Select(jt => new JobTitleViewModel
          {
            Id = jt.Id,
            Name = jt.Name,
            CreatedAt = jt.CreatedAt,
            UsageCount = 0 // Simplified - not tracking usage
          })
          .ToListAsync();

      return View(jobTitles);
    }

    // POST: Settings/CreateJobTitle
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateJobTitle(JobTitleViewModel model)
    {
      if (!ModelState.IsValid)
      {
        TempData["ErrorMessage"] = "Invalid data provided.";
        return RedirectToAction(nameof(JobTitles));
      }

      try
      {
        // Check for duplicates
        var exists = await _context.JobTitles
            .AnyAsync(jt => jt.Name.ToLower() == model.Name.ToLower() && jt.DeletedAt == null);

        if (exists)
        {
          TempData["ErrorMessage"] = "A job title with this name already exists.";
          return RedirectToAction(nameof(JobTitles));
        }

        var jobTitle = new JobTitle
        {
          Id = Guid.NewGuid(),
          Name = model.Name,
          CreatedAt = DateTimeOffset.UtcNow
        };

        _context.JobTitles.Add(jobTitle);
        await _context.SaveChangesAsync();

        _logger.LogInformation("JobTitle {JobTitleId} created by admin {AdminEmail}",
            jobTitle.Id, User.Identity?.Name);
        TempData["SuccessMessage"] = "Job title created successfully.";

        return RedirectToAction(nameof(JobTitles));
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error creating job title");
        TempData["ErrorMessage"] = "Unable to create job title. Please try again.";
        return RedirectToAction(nameof(JobTitles));
      }
    }

    // POST: Settings/UpdateJobTitle
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateJobTitle(Guid id, JobTitleViewModel model)
    {
      if (!ModelState.IsValid)
      {
        TempData["ErrorMessage"] = "Invalid data provided.";
        return RedirectToAction(nameof(JobTitles));
      }

      try
      {
        var jobTitle = await _context.JobTitles
            .FirstOrDefaultAsync(jt => jt.Id == id && jt.DeletedAt == null);

        if (jobTitle == null)
        {
          TempData["ErrorMessage"] = "Job title not found.";
          return RedirectToAction(nameof(JobTitles));
        }

        // Check for duplicates (excluding current)
        var duplicateExists = await _context.JobTitles
            .AnyAsync(jt => jt.Id != id &&
                jt.Name.ToLower() == model.Name.ToLower() &&
                jt.DeletedAt == null);

        if (duplicateExists)
        {
          TempData["ErrorMessage"] = "A job title with this name already exists.";
          return RedirectToAction(nameof(JobTitles));
        }

        jobTitle.Name = model.Name;
        jobTitle.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("JobTitle {JobTitleId} updated by admin {AdminEmail}",
            id, User.Identity?.Name);
        TempData["SuccessMessage"] = "Job title updated successfully.";

        return RedirectToAction(nameof(JobTitles));
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error updating job title {JobTitleId}", id);
        TempData["ErrorMessage"] = "Unable to update job title. Please try again.";
        return RedirectToAction(nameof(JobTitles));
      }
    }

    // POST: Settings/DeleteJobTitle
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteJobTitle(Guid id)
    {
      try
      {
        var jobTitle = await _context.JobTitles
            .FirstOrDefaultAsync(jt => jt.Id == id && jt.DeletedAt == null);

        if (jobTitle == null)
        {
          TempData["ErrorMessage"] = "Job title not found.";
          return RedirectToAction(nameof(JobTitles));
        }

        // Soft delete
        jobTitle.DeletedAt = DateTimeOffset.UtcNow;
        jobTitle.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("JobTitle {JobTitleId} soft deleted by admin {AdminEmail}",
            id, User.Identity?.Name);
        TempData["SuccessMessage"] = "Job title deleted successfully.";

        return RedirectToAction(nameof(JobTitles));
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error deleting job title {JobTitleId}", id);
        TempData["ErrorMessage"] = "Unable to delete job title. Please try again.";
        return RedirectToAction(nameof(JobTitles));
      }
    }

    #endregion

    #region Employment Types

    // GET: Settings/EmploymentTypes
    public async Task<IActionResult> EmploymentTypes()
    {
      var employmentTypes = await _context.EmploymentTypes
          .Where(et => et.DeletedAt == null)
          .OrderBy(et => et.Name)
          .Select(et => new EmploymentTypeViewModel
          {
            Id = et.Id,
            Name = et.Name,
            CreatedAt = et.CreatedAt,
            UsageCount = 0 // Simplified - not tracking usage
          })
          .ToListAsync();

      return View(employmentTypes);
    }

    // POST: Settings/CreateEmploymentType
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEmploymentType(EmploymentTypeViewModel model)
    {
      if (!ModelState.IsValid)
      {
        TempData["ErrorMessage"] = "Invalid data provided.";
        return RedirectToAction(nameof(EmploymentTypes));
      }

      try
      {
        // Check for duplicates
        var exists = await _context.EmploymentTypes
            .AnyAsync(et => et.Name.ToLower() == model.Name.ToLower() && et.DeletedAt == null);

        if (exists)
        {
          TempData["ErrorMessage"] = "An employment type with this name already exists.";
          return RedirectToAction(nameof(EmploymentTypes));
        }

        var employmentType = new EmploymentType
        {
          Id = Guid.NewGuid(),
          Name = model.Name,
          CreatedAt = DateTimeOffset.UtcNow
        };

        _context.EmploymentTypes.Add(employmentType);
        await _context.SaveChangesAsync();

        _logger.LogInformation("EmploymentType {EmploymentTypeId} created by admin {AdminEmail}",
            employmentType.Id, User.Identity?.Name);
        TempData["SuccessMessage"] = "Employment type created successfully.";

        return RedirectToAction(nameof(EmploymentTypes));
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error creating employment type");
        TempData["ErrorMessage"] = "Unable to create employment type. Please try again.";
        return RedirectToAction(nameof(EmploymentTypes));
      }
    }

    // POST: Settings/UpdateEmploymentType
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateEmploymentType(Guid id, EmploymentTypeViewModel model)
    {
      if (!ModelState.IsValid)
      {
        TempData["ErrorMessage"] = "Invalid data provided.";
        return RedirectToAction(nameof(EmploymentTypes));
      }

      try
      {
        var employmentType = await _context.EmploymentTypes
            .FirstOrDefaultAsync(et => et.Id == id && et.DeletedAt == null);

        if (employmentType == null)
        {
          TempData["ErrorMessage"] = "Employment type not found.";
          return RedirectToAction(nameof(EmploymentTypes));
        }

        // Check for duplicates (excluding current)
        var duplicateExists = await _context.EmploymentTypes
            .AnyAsync(et => et.Id != id &&
                et.Name.ToLower() == model.Name.ToLower() &&
                et.DeletedAt == null);

        if (duplicateExists)
        {
          TempData["ErrorMessage"] = "An employment type with this name already exists.";
          return RedirectToAction(nameof(EmploymentTypes));
        }

        employmentType.Name = model.Name;
        employmentType.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("EmploymentType {EmploymentTypeId} updated by admin {AdminEmail}",
            id, User.Identity?.Name);
        TempData["SuccessMessage"] = "Employment type updated successfully.";

        return RedirectToAction(nameof(EmploymentTypes));
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error updating employment type {EmploymentTypeId}", id);
        TempData["ErrorMessage"] = "Unable to update employment type. Please try again.";
        return RedirectToAction(nameof(EmploymentTypes));
      }
    }

    // POST: Settings/DeleteEmploymentType
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteEmploymentType(Guid id)
    {
      try
      {
        var employmentType = await _context.EmploymentTypes
            .FirstOrDefaultAsync(et => et.Id == id && et.DeletedAt == null);

        if (employmentType == null)
        {
          TempData["ErrorMessage"] = "Employment type not found.";
          return RedirectToAction(nameof(EmploymentTypes));
        }

        // Soft delete
        employmentType.DeletedAt = DateTimeOffset.UtcNow;
        employmentType.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("EmploymentType {EmploymentTypeId} soft deleted by admin {AdminEmail}",
            id, User.Identity?.Name);
        TempData["SuccessMessage"] = "Employment type deleted successfully.";

        return RedirectToAction(nameof(EmploymentTypes));
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error deleting employment type {EmploymentTypeId}", id);
        TempData["ErrorMessage"] = "Unable to delete employment type. Please try again.";
        return RedirectToAction(nameof(EmploymentTypes));
      }
    }

    #endregion
  }
}
