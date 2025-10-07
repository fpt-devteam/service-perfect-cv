using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using ServicePerfectCV.Application.DTOs.Authentication.Requests;
using ServicePerfectCV.Application.Exceptions;
using ServicePerfectCV.Application.Services;
using ServicePerfectCV.WebAdmin.Models;
using System.Security.Claims;

namespace ServicePerfectCV.WebAdmin.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            AuthService authService,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            // If already authenticated, redirect to home
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Use AuthService to authenticate admin
                var loginRequest = new LoginRequest
                {
                    Email = model.Email,
                    Password = model.Password
                };

                // LoginAdminAsync handles all validation including admin role check
                var user = await _authService.LoginAdminAsync(loginRequest);

                // Create session-based authentication claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                    new Claim("FullName", $"{user.FirstName ?? ""} {user.LastName ?? ""}".Trim())
                };

                if (string.IsNullOrWhiteSpace(claims.First(c => c.Type == "FullName").Value))
                {
                    claims.RemoveAll(c => c.Type == "FullName");
                    claims.Add(new Claim("FullName", user.Email));
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = model.RememberMe 
                        ? DateTimeOffset.UtcNow.AddDays(30) 
                        : DateTimeOffset.UtcNow.AddHours(8)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                _logger.LogInformation("Admin user {Email} logged in successfully", model.Email);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }
            catch (DomainException ex)
            {
                _logger.LogWarning("Login failed for {Email}: {Error}", model.Email, ex.Message);
                
                // Map domain exceptions to user-friendly messages
                var errorMessage = ex.Error.Code switch
                {
                    "InvalidCredential" => "Invalid email or password",
                    "AccountNotActivated" => "Your account is not activated. Please check your email.",
                    "AccountExistsWithDifferentMethod" => "This account uses a different login method",
                    "AccessDenied" => "Access denied. Admin privileges required.",
                    _ => "Login failed. Please try again."
                };

                ModelState.AddModelError(string.Empty, errorMessage);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during login for {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            _logger.LogInformation("Admin user {UserId} logged out", userId);
            
            return RedirectToAction("Login", "Auth");
        }
    }
}

