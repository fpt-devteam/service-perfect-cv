using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServicePerfectCV.Application.Services;
using ServicePerfectCV.Domain.Constants;
using ServicePerfectCV.Infrastructure.Data;
using ServicePerfectCV.WebAdmin.Models.Users;
using ServicePerfectCV.WebAdmin.Models.Shared;
using ServicePerfectCV.WebAdmin.Services;

namespace ServicePerfectCV.WebAdmin.Controllers
{
  [Authorize]
  public class UsersController : Controller
  {
    private readonly ApplicationDbContext _context;
    private readonly UserService _userService;
    private readonly ExportService _exportService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        ApplicationDbContext context,
        UserService userService,
        ExportService exportService,
        ILogger<UsersController> logger)
    {
      _context = context;
      _userService = userService;
      _exportService = exportService;
      _logger = logger;
    }

    // GET: Users
    public async Task<IActionResult> Index(string? search, UserStatus? status, UserRole? role, int page = 1, int pageSize = 20)
    {
      ViewData["CurrentSearch"] = search;
      ViewData["CurrentStatus"] = status;
      ViewData["CurrentRole"] = role;
      ViewData["CurrentPage"] = page;

      var query = _context.Users
          .Where(u => u.DeletedAt == null)
          .AsQueryable();

      // Apply filters
      if (!string.IsNullOrWhiteSpace(search))
      {
        search = search.ToLower();
        query = query.Where(u =>
            u.Email.ToLower().Contains(search) ||
            (u.FirstName != null && u.FirstName.ToLower().Contains(search)) ||
            (u.LastName != null && u.LastName.ToLower().Contains(search)));
      }

      if (status.HasValue)
      {
        query = query.Where(u => u.Status == status.Value);
      }

      if (role.HasValue)
      {
        query = query.Where(u => u.Role == role.Value);
      }

      var totalCount = await query.CountAsync();
      var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

      var users = await query
          .OrderByDescending(u => u.CreatedAt)
          .Skip((page - 1) * pageSize)
          .Take(pageSize)
          .Select(u => new UserListViewModel
          {
            Id = u.Id,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            AvatarUrl = u.AvatarUrl,
            Status = u.Status,
            Role = u.Role,
            TotalCredit = u.TotalCredit,
            UsedCredit = u.UsedCredit,
            CreatedAt = u.CreatedAt,
            UpdatedAt = u.UpdatedAt,
            CVCount = u.CVs.Count(cv => cv.DeletedAt == null)
          })
          .ToListAsync();

      ViewBag.TotalPages = totalPages;
      ViewBag.TotalCount = totalCount;

      return View(users);
    }

    // GET: Users/Details/5
    public async Task<IActionResult> Details(Guid id)
    {
      var user = await _context.Users
          .Include(u => u.CVs.Where(cv => cv.DeletedAt == null))
          .Include(u => u.BillingHistories).ThenInclude(b => b.Package)
          .Include(u => u.DeviceTokens)
          .FirstOrDefaultAsync(u => u.Id == id && u.DeletedAt == null);

      if (user == null)
      {
        TempData["ErrorMessage"] = "User not found.";
        return RedirectToAction(nameof(Index));
      }

      var viewModel = new UserDetailViewModel
      {
        Id = user.Id,
        Email = user.Email,
        FirstName = user.FirstName,
        LastName = user.LastName,
        AvatarUrl = user.AvatarUrl,
        Status = user.Status,
        Role = user.Role,
        AuthMethod = user.AuthMethod,
        TotalCredit = user.TotalCredit,
        UsedCredit = user.UsedCredit,
        CreatedAt = user.CreatedAt,
        UpdatedAt = user.UpdatedAt,
        DeletedAt = user.DeletedAt,
        CVCount = user.CVs.Count,
        DeviceCount = user.DeviceTokens.Count,
        TotalSpent = user.BillingHistories.Sum(b => b.Amount),
        RecentCVs = user.CVs
              .OrderByDescending(cv => cv.CreatedAt)
              .Take(5)
              .Select(cv => new UserCVViewModel
              {
                Id = cv.Id,
                Title = cv.Title,
                CreatedAt = cv.CreatedAt,
                IsStructuredDone = cv.IsStructuredDone
              })
              .ToList(),
        RecentBillings = user.BillingHistories
              .OrderByDescending(b => b.CreatedAt)
              .Take(10)
              .Select(b => new UserBillingViewModel
              {
                Id = b.Id,
                Amount = b.Amount,
                Description = $"Package: {b.Package.Name} - {b.Status}",
                CreatedAt = b.CreatedAt
              })
              .ToList()
      };

      return View(viewModel);
    }

    // GET: Users/Edit/5
    public async Task<IActionResult> Edit(Guid id)
    {
      var user = await _context.Users.FindAsync(id);
      if (user == null || user.DeletedAt != null)
      {
        TempData["ErrorMessage"] = "User not found.";
        return RedirectToAction(nameof(Index));
      }

      var viewModel = new EditUserViewModel
      {
        Id = user.Id,
        Email = user.Email,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Status = user.Status,
        Role = user.Role,
        TotalCredit = user.TotalCredit,
        UsedCredit = user.UsedCredit
      };

      return View(viewModel);
    }

    // POST: Users/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, EditUserViewModel model)
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
        var user = await _context.Users.FindAsync(id);
        if (user == null || user.DeletedAt != null)
        {
          TempData["ErrorMessage"] = "User not found.";
          return RedirectToAction(nameof(Index));
        }

        // Update user properties
        user.Email = model.Email;
        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.Status = model.Status;
        user.Role = model.Role;
        user.TotalCredit = model.TotalCredit;
        user.UsedCredit = model.UsedCredit;
        user.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} updated by admin {AdminEmail}", id, User.Identity?.Name);
        TempData["SuccessMessage"] = "User updated successfully.";

        return RedirectToAction(nameof(Details), new { id });
      }
      catch (DbUpdateException ex)
      {
        _logger.LogError(ex, "Error updating user {UserId}", id);
        ModelState.AddModelError("", "Unable to save changes. Please try again.");
        return View(model);
      }
    }

    // POST: Users/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
      try
      {
        var user = await _context.Users.FindAsync(id);
        if (user == null || user.DeletedAt != null)
        {
          TempData["ErrorMessage"] = "User not found.";
          return RedirectToAction(nameof(Index));
        }

        // Soft delete
        user.DeletedAt = DateTimeOffset.UtcNow;
        user.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} soft deleted by admin {AdminEmail}", id, User.Identity?.Name);
        TempData["SuccessMessage"] = "User deleted successfully.";

        return RedirectToAction(nameof(Index));
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error deleting user {UserId}", id);
        TempData["ErrorMessage"] = "Unable to delete user. Please try again.";
        return RedirectToAction(nameof(Index));
      }
    }

    // POST: Users/ActivateAccount/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ActivateAccount(Guid id)
    {
      try
      {
        var user = await _context.Users.FindAsync(id);
        if (user == null || user.DeletedAt != null)
        {
          TempData["ErrorMessage"] = "User not found.";
          return RedirectToAction(nameof(Index));
        }

        if (user.Status == UserStatus.Active)
        {
          TempData["InfoMessage"] = "User account is already active.";
          return RedirectToAction(nameof(Details), new { id });
        }

        user.Status = UserStatus.Active;
        user.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} activated by admin {AdminEmail}", id, User.Identity?.Name);
        TempData["SuccessMessage"] = "User account activated successfully.";

        return RedirectToAction(nameof(Details), new { id });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error activating user {UserId}", id);
        TempData["ErrorMessage"] = "Unable to activate user. Please try again.";
        return RedirectToAction(nameof(Details), new { id });
      }
    }

    // POST: Users/DeactivateAccount/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeactivateAccount(Guid id)
    {
      try
      {
        var user = await _context.Users.FindAsync(id);
        if (user == null || user.DeletedAt != null)
        {
          TempData["ErrorMessage"] = "User not found.";
          return RedirectToAction(nameof(Index));
        }

        if (user.Status == UserStatus.Inactive)
        {
          TempData["InfoMessage"] = "User account is already inactive.";
          return RedirectToAction(nameof(Details), new { id });
        }

        user.Status = UserStatus.Inactive;
        user.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} deactivated by admin {AdminEmail}", id, User.Identity?.Name);
        TempData["SuccessMessage"] = "User account deactivated successfully.";

        return RedirectToAction(nameof(Details), new { id });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error deactivating user {UserId}", id);
        TempData["ErrorMessage"] = "Unable to deactivate user. Please try again.";
        return RedirectToAction(nameof(Details), new { id });
      }
    }

    // POST: Users/AddCredits/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddCredits(Guid id, int credits)
    {
      if (credits <= 0)
      {
        TempData["ErrorMessage"] = "Credits must be greater than zero.";
        return RedirectToAction(nameof(Details), new { id });
      }

      try
      {
        var user = await _context.Users.FindAsync(id);
        if (user == null || user.DeletedAt != null)
        {
          TempData["ErrorMessage"] = "User not found.";
          return RedirectToAction(nameof(Index));
        }

        user.TotalCredit += credits;
        user.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Added {Credits} credits to user {UserId} by admin {AdminEmail}",
            credits, id, User.Identity?.Name);
        TempData["SuccessMessage"] = $"Successfully added {credits} credits to user account.";

        return RedirectToAction(nameof(Details), new { id });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error adding credits to user {UserId}", id);
        TempData["ErrorMessage"] = "Unable to add credits. Please try again.";
        return RedirectToAction(nameof(Details), new { id });
      }
    }

    // GET: Users/Export
    public async Task<IActionResult> Export(string? search, UserStatus? status, UserRole? role)
    {
      try
      {
        var query = _context.Users
            .Where(u => u.DeletedAt == null)
            .AsQueryable();

        // Apply same filters as Index
        if (!string.IsNullOrWhiteSpace(search))
        {
          search = search.ToLower();
          query = query.Where(u =>
              u.Email.ToLower().Contains(search) ||
              (u.FirstName != null && u.FirstName.ToLower().Contains(search)) ||
              (u.LastName != null && u.LastName.ToLower().Contains(search)));
        }

        if (status.HasValue)
        {
          query = query.Where(u => u.Status == status.Value);
        }

        if (role.HasValue)
        {
          query = query.Where(u => u.Role == role.Value);
        }

        var users = await query
            .OrderByDescending(u => u.CreatedAt)
            .Select(u => new UserListViewModel
            {
              Id = u.Id,
              Email = u.Email,
              FirstName = u.FirstName,
              LastName = u.LastName,
              AvatarUrl = u.AvatarUrl,
              Role = u.Role,
              Status = u.Status,
              TotalCredit = u.TotalCredit,
              UsedCredit = u.UsedCredit,
              CVCount = _context.CVs.Count(cv => cv.UserId == u.Id && cv.DeletedAt == null),
              CreatedAt = u.CreatedAt,
              UpdatedAt = u.UpdatedAt
            })
            .ToListAsync();

        var csvBytes = _exportService.ExportUsersToCsv(users);
        var fileName = $"users_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

        _logger.LogInformation("Admin {AdminEmail} exported {Count} users to CSV",
            User.Identity?.Name, users.Count);

        return File(csvBytes, "text/csv", fileName);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error exporting users");
        TempData["ErrorMessage"] = "Unable to export users. Please try again.";
        return RedirectToAction(nameof(Index));
      }
    }

    // POST: Users/BulkDelete
    [HttpPost]
    public async Task<IActionResult> BulkDelete([FromBody] BulkDeleteRequest request)
    {
      try
      {
        if (request?.Ids == null || !request.Ids.Any())
        {
          return BadRequest("No users selected for deletion");
        }

        var users = await _context.Users
            .Where(u => request.Ids.Contains(u.Id) && u.DeletedAt == null)
            .ToListAsync();

        if (!users.Any())
        {
          return NotFound("No valid users found for deletion");
        }

        foreach (var user in users)
        {
          user.DeletedAt = DateTimeOffset.UtcNow;
          user.UpdatedAt = DateTimeOffset.UtcNow;
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Admin {AdminEmail} bulk deleted {Count} users: {UserIds}",
            User.Identity?.Name, users.Count, string.Join(", ", request.Ids));

        return Ok(new { message = $"Successfully deleted {users.Count} user(s)" });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error bulk deleting users");
        return StatusCode(500, "Error deleting users. Please try again.");
      }
    }
  }
}

