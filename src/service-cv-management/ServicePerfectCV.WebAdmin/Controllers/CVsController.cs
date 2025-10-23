using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServicePerfectCV.Application.Services;
using ServicePerfectCV.Infrastructure.Data;
using ServicePerfectCV.WebAdmin.Models.CVs;
using ServicePerfectCV.WebAdmin.Models.Shared;
using ServicePerfectCV.WebAdmin.Services;

namespace ServicePerfectCV.WebAdmin.Controllers
{
  [Authorize]
  public class CVsController : Controller
  {
    private readonly ApplicationDbContext _context;
    private readonly ExportService _exportService;
    private readonly ILogger<CVsController> _logger;

    public CVsController(
        ApplicationDbContext context,
        ExportService exportService,
        ILogger<CVsController> logger)
    {
      _context = context;
      _exportService = exportService;
      _logger = logger;
    }

    // GET: CVs
    public async Task<IActionResult> Index(string? search, bool? isStructuredDone, int page = 1, int pageSize = 20)
    {
      ViewData["CurrentSearch"] = search;
      ViewData["CurrentStructured"] = isStructuredDone;
      ViewData["CurrentPage"] = page;

      var query = _context.CVs
          .Include(cv => cv.User)
          .Where(cv => cv.DeletedAt == null)
          .AsQueryable();

      // Apply filters
      if (!string.IsNullOrWhiteSpace(search))
      {
        search = search.ToLower();
        query = query.Where(cv =>
            cv.Title.ToLower().Contains(search) ||
            cv.User.Email.ToLower().Contains(search) ||
            (cv.User.FirstName != null && cv.User.FirstName.ToLower().Contains(search)) ||
            (cv.User.LastName != null && cv.User.LastName.ToLower().Contains(search)));
      }

      if (isStructuredDone.HasValue)
      {
        query = query.Where(cv => cv.IsStructuredDone == isStructuredDone.Value);
      }

      var totalCount = await query.CountAsync();
      var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

      var cvs = await query
          .OrderByDescending(cv => cv.CreatedAt)
          .Skip((page - 1) * pageSize)
          .Take(pageSize)
          .Select(cv => new CVListViewModel
          {
            Id = cv.Id,
            Title = cv.Title,
            UserEmail = cv.User.Email,
            UserFullName = (cv.User.FirstName ?? "") + " " + (cv.User.LastName ?? ""),
            IsStructuredDone = cv.IsStructuredDone,
            CreatedAt = cv.CreatedAt,
            UpdatedAt = cv.UpdatedAt
          })
          .ToListAsync();

      ViewBag.TotalPages = totalPages;
      ViewBag.TotalCount = totalCount;

      return View(cvs);
    }

    // GET: CVs/Details/5
    public async Task<IActionResult> Details(Guid id)
    {
      try
      {
        var cv = await _context.CVs
            .Include(cv => cv.User)
            .Include(cv => cv.Contact)
            .Include(cv => cv.Summary)
            .Include(cv => cv.Educations)
            .Include(cv => cv.Experiences).ThenInclude(e => e.EmploymentType)
            .Include(cv => cv.Skills)
            .Include(cv => cv.Projects)
            .Include(cv => cv.Certifications)
            .FirstOrDefaultAsync(cv => cv.Id == id && cv.DeletedAt == null);

        if (cv == null)
        {
          TempData["ErrorMessage"] = "CV not found.";
          return RedirectToAction(nameof(Index));
        }

        // Simplified ViewModel - only use properties that actually exist
        var viewModel = new CVDetailViewModel
        {
          Id = cv.Id,
          UserId = cv.UserId,
          UserEmail = cv.User.Email,
          Title = cv.Title,
          VersionId = cv.VersionId,
          IsStructuredDone = cv.IsStructuredDone,
          CreatedAt = cv.CreatedAt,
          UpdatedAt = cv.UpdatedAt,
          PdfFileName = cv.PdfFileName,
          ExtractedText = cv.ExtractedText,
          Contact = cv.Contact != null ? new Application.DTOs.Contact.Responses.ContactResponse
          {
            Id = cv.Contact.Id,
            CVId = cv.Contact.CVId,
            Email = cv.Contact.Email,
            PhoneNumber = cv.Contact.PhoneNumber,
            LinkedInUrl = cv.Contact.LinkedInUrl,
            GitHubUrl = cv.Contact.GitHubUrl,
            PersonalWebsiteUrl = cv.Contact.PersonalWebsiteUrl,
            Country = cv.Contact.Country,
            City = cv.Contact.City
          } : null,
          Summary = cv.Summary != null ? new Application.DTOs.Summary.Responses.SummaryResponse
          {
            Id = cv.Summary.Id,
            Content = cv.Summary.Content
          } : null,
          Educations = cv.Educations.Where(e => e.DeletedAt == null).Select(e => new Application.DTOs.Education.Responses.EducationResponse
          {
            Id = e.Id,
            Degree = e.Degree,
            Organization = e.Organization,
            FieldOfStudy = e.FieldOfStudy,
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            Description = e.Description,
            Gpa = e.Gpa
          }).ToList(),
          Experiences = cv.Experiences.Where(e => e.DeletedAt == null).Select(e => new Application.DTOs.Experience.Responses.ExperienceResponse
          {
            Id = e.Id,
            CVId = e.CVId,
            JobTitle = e.JobTitle,
            Organization = e.Organization,
            EmploymentTypeId = e.EmploymentTypeId,
            EmploymentTypeName = e.EmploymentType?.Name ?? "Unknown",
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            Description = e.Description,
            Location = e.Location,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt
          }).ToList(),
          Projects = cv.Projects.Where(p => p.DeletedAt == null).Select(p => new Application.DTOs.Project.Responses.ProjectResponse
          {
            Id = p.Id,
            CVId = p.CVId,
            Title = p.Title,
            Description = p.Description,
            StartDate = p.StartDate,
            EndDate = p.EndDate,
            Link = p.Link,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
          }).ToList(),
          Skills = cv.Skills.Where(s => s.DeletedAt == null).Select(s => new Application.DTOs.Skill.Responses.SkillResponse
          {
            Id = s.Id,
            Category = s.Category,
            Content = s.Content,
            CreatedAt = s.CreatedAt,
            UpdatedAt = s.UpdatedAt
          }).ToList(),
          Certifications = cv.Certifications.Where(c => c.DeletedAt == null).Select(c => new Application.DTOs.Certification.Responses.CertificationResponse
          {
            Id = c.Id,
            CVId = c.CVId,
            Name = c.Name,
            Organization = c.Organization,
            IssuedDate = c.IssuedDate,
            Description = c.Description
          }).ToList()
        };

        return View(viewModel);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error loading CV details for {CVId}", id);
        TempData["ErrorMessage"] = "Error loading CV details.";
        return RedirectToAction(nameof(Index));
      }
    }

    // POST: CVs/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
      try
      {
        var cv = await _context.CVs
            .FirstOrDefaultAsync(cv => cv.Id == id && cv.DeletedAt == null);

        if (cv == null)
        {
          TempData["ErrorMessage"] = "CV not found.";
          return RedirectToAction(nameof(Index));
        }

        // Soft delete
        cv.DeletedAt = DateTimeOffset.UtcNow;
        cv.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("CV {CVId} soft deleted by admin {AdminEmail}",
            id, User.Identity?.Name);
        TempData["SuccessMessage"] = "CV deleted successfully.";

        return RedirectToAction(nameof(Index));
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error deleting CV {CVId}", id);
        TempData["ErrorMessage"] = "Unable to delete CV. Please try again.";
        return RedirectToAction(nameof(Index));
      }
    }

    // GET: CVs/Export
    public async Task<IActionResult> Export(string? search, bool? isStructuredDone)
    {
      try
      {
        var query = _context.CVs
            .Include(cv => cv.User)
            .Where(cv => cv.DeletedAt == null)
            .AsQueryable();

        // Apply same filters as Index
        if (!string.IsNullOrWhiteSpace(search))
        {
          search = search.ToLower();
          query = query.Where(cv =>
              cv.Title.ToLower().Contains(search) ||
              cv.User.Email.ToLower().Contains(search) ||
              (cv.User.FirstName != null && cv.User.FirstName.ToLower().Contains(search)) ||
              (cv.User.LastName != null && cv.User.LastName.ToLower().Contains(search)));
        }

        if (isStructuredDone.HasValue)
        {
          query = query.Where(cv => cv.IsStructuredDone == isStructuredDone.Value);
        }

        var cvs = await query
            .OrderByDescending(cv => cv.CreatedAt)
            .Select(cv => new CVListViewModel
            {
              Id = cv.Id,
              Title = cv.Title,
              UserId = cv.UserId,
              UserEmail = cv.User.Email,
              UserFullName = (cv.User.FirstName ?? "") + " " + (cv.User.LastName ?? ""),
              IsStructuredDone = cv.IsStructuredDone,
              HasPdf = !string.IsNullOrEmpty(cv.PdfFileName),
              CreatedAt = cv.CreatedAt,
              UpdatedAt = cv.UpdatedAt,
              EducationCount = _context.Educations.Count(e => e.CVId == cv.Id && e.DeletedAt == null),
              ExperienceCount = _context.Experiences.Count(e => e.CVId == cv.Id && e.DeletedAt == null),
              SkillCount = _context.Skills.Count(s => s.CVId == cv.Id && s.DeletedAt == null),
              ProjectCount = _context.Projects.Count(p => p.CVId == cv.Id && p.DeletedAt == null),
              CertificationCount = _context.Certifications.Count(c => c.CVId == cv.Id && c.DeletedAt == null)
            })
            .ToListAsync();

        var csvBytes = _exportService.ExportCVsToCsv(cvs);
        var fileName = $"cvs_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

        _logger.LogInformation("Admin {AdminEmail} exported {Count} CVs to CSV",
            User.Identity?.Name, cvs.Count);

        return File(csvBytes, "text/csv", fileName);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error exporting CVs");
        TempData["ErrorMessage"] = "Unable to export CVs. Please try again.";
        return RedirectToAction(nameof(Index));
      }
    }

    // POST: CVs/BulkDelete
    [HttpPost]
    public async Task<IActionResult> BulkDelete([FromBody] BulkDeleteRequest request)
    {
      try
      {
        if (request?.Ids == null || !request.Ids.Any())
        {
          return BadRequest("No CVs selected for deletion");
        }

        var cvs = await _context.CVs
            .Where(cv => request.Ids.Contains(cv.Id) && cv.DeletedAt == null)
            .ToListAsync();

        if (!cvs.Any())
        {
          return NotFound("No valid CVs found for deletion");
        }

        foreach (var cv in cvs)
        {
          cv.DeletedAt = DateTimeOffset.UtcNow;
          cv.UpdatedAt = DateTimeOffset.UtcNow;
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Admin {AdminEmail} bulk deleted {Count} CVs: {CVIds}",
            User.Identity?.Name, cvs.Count, string.Join(", ", request.Ids));

        return Ok(new { message = $"Successfully deleted {cvs.Count} CV(s)" });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error bulk deleting CVs");
        return StatusCode(500, "Error deleting CVs. Please try again.");
      }
    }
  }
}
