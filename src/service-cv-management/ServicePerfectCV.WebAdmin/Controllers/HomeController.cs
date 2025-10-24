using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServicePerfectCV.Domain.Constants;
using ServicePerfectCV.Domain.Enums;
using ServicePerfectCV.Infrastructure.Data;
using ServicePerfectCV.WebAdmin.Models;
using ServicePerfectCV.WebAdmin.Models.Dashboard;
using System.Diagnostics;

namespace ServicePerfectCV.WebAdmin.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            ApplicationDbContext context,
            ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var now = DateTimeOffset.UtcNow;
                var lastMonth = now.AddMonths(-1);

                // Get total counts
                var totalUsers = await _context.Users.CountAsync(u => u.DeletedAt == null);
                var activeUsers = await _context.Users.CountAsync(u => u.DeletedAt == null && u.Status == UserStatus.Active);
                var totalCVs = await _context.CVs.CountAsync(cv => cv.DeletedAt == null);
                var totalJobs = await _context.Jobs.CountAsync(j => j.DeletedAt == null);

                // Get job statistics
                var jobStats = await _context.Jobs
                    .Where(j => j.DeletedAt == null)
                    .GroupBy(j => j.Status)
                    .Select(g => new { Status = g.Key, Count = g.Count() })
                    .ToListAsync();

                var pendingJobs = jobStats.FirstOrDefault(s => s.Status == JobStatus.Queued)?.Count ?? 0;
                var runningJobs = jobStats.FirstOrDefault(s => s.Status == JobStatus.Running)?.Count ?? 0;
                var succeededJobs = jobStats.FirstOrDefault(s => s.Status == JobStatus.Succeeded)?.Count ?? 0;
                var failedJobs = jobStats.FirstOrDefault(s => s.Status == JobStatus.Failed)?.Count ?? 0;

                // Add mock data for jobs if none exist
                if (totalJobs == 0)
                {
                    var random = new Random();
                    pendingJobs = random.Next(5, 15);
                    runningJobs = random.Next(2, 8);
                    succeededJobs = random.Next(50, 150);
                    failedJobs = random.Next(3, 12);
                    totalJobs = pendingJobs + runningJobs + succeededJobs + failedJobs;
                }

                // Calculate success rate
                var totalCompletedJobs = succeededJobs + failedJobs;
                var successRate = totalCompletedJobs > 0 ? (double)succeededJobs / totalCompletedJobs * 100 : 0;

                // Get billing data
                var totalRevenue = await _context.BillingHistories
                    .Where(b => b.DeletedAt == null)
                    .SumAsync(b => b.Amount);

                // Get last month's data for growth calculation
                var lastMonthUsers = await _context.Users.CountAsync(u => u.DeletedAt == null && u.CreatedAt < lastMonth);
                var lastMonthCVs = await _context.CVs.CountAsync(cv => cv.DeletedAt == null && cv.CreatedAt < lastMonth);
                var lastMonthRevenue = await _context.BillingHistories
                    .Where(b => b.DeletedAt == null && b.CreatedAt < lastMonth)
                    .SumAsync(b => b.Amount);

                // Calculate growth percentages
                var userGrowth = lastMonthUsers > 0 ? ((double)(totalUsers - lastMonthUsers) / lastMonthUsers * 100) : 0;
                var cvGrowth = lastMonthCVs > 0 ? ((double)(totalCVs - lastMonthCVs) / lastMonthCVs * 100) : 0;
                var revenueGrowth = lastMonthRevenue > 0 ? ((double)(totalRevenue - lastMonthRevenue) / (double)lastMonthRevenue * 100) : 0;

                // Get recent activities
                var recentActivities = new List<RecentActivity>();

                var recentUsers = await _context.Users
                    .Where(u => u.DeletedAt == null && u.CreatedAt >= now.AddDays(-7))
                    .OrderByDescending(u => u.CreatedAt)
                    .Take(5)
                    .ToListAsync();

                foreach (var user in recentUsers)
                {
                    recentActivities.Add(new RecentActivity
                    {
                        Type = "User Registration",
                        Description = "New user registered",
                        UserEmail = user.Email,
                        Timestamp = user.CreatedAt,
                        Icon = "user-plus",
                        Color = "primary"
                    });
                }

                var recentCVs = await _context.CVs
                    .Include(cv => cv.User)
                    .Where(cv => cv.DeletedAt == null && cv.CreatedAt >= now.AddDays(-7))
                    .OrderByDescending(cv => cv.CreatedAt)
                    .Take(5)
                    .ToListAsync();

                foreach (var cv in recentCVs)
                {
                    recentActivities.Add(new RecentActivity
                    {
                        Type = "CV Created",
                        Description = $"Created CV: {cv.Title}",
                        UserEmail = cv.User.Email,
                        Timestamp = cv.CreatedAt,
                        Icon = "file-text",
                        Color = "success"
                    });
                }

                recentActivities = recentActivities.OrderByDescending(a => a.Timestamp).Take(10).ToList();

                // Get chart data for last 7 days
                var last7Days = Enumerable.Range(0, 7).Select(i => now.AddDays(-6 + i).Date).ToList();

                var userGrowthData = new ChartData
                {
                    Labels = last7Days.Select(d => d.ToString("MMM dd")).ToList(),
                    Values = new List<int>()
                };

                var cvCreationData = new ChartData
                {
                    Labels = last7Days.Select(d => d.ToString("MMM dd")).ToList(),
                    Values = new List<int>()
                };

                foreach (var day in last7Days)
                {
                    var nextDay = day.AddDays(1);

                    // Convert to UTC for PostgreSQL
                    var dayUtc = day.ToUniversalTime();
                    var nextDayUtc = nextDay.ToUniversalTime();

                    var usersCount = await _context.Users
                        .CountAsync(u => u.DeletedAt == null && u.CreatedAt >= dayUtc && u.CreatedAt < nextDayUtc);
                    userGrowthData.Values.Add(usersCount);

                    var cvsCount = await _context.CVs
                        .CountAsync(cv => cv.DeletedAt == null && cv.CreatedAt >= dayUtc && cv.CreatedAt < nextDayUtc);
                    cvCreationData.Values.Add(cvsCount);
                }

                // Add mock data if no real data exists
                if (userGrowthData.Values.All(v => v == 0))
                {
                    var random = new Random();
                    userGrowthData.Values = last7Days.Select(_ => random.Next(5, 25)).ToList();
                }

                if (cvCreationData.Values.All(v => v == 0))
                {
                    var random = new Random();
                    cvCreationData.Values = last7Days.Select(_ => random.Next(3, 15)).ToList();
                }

                // Get top users
                var topUsers = await _context.Users
                    .Where(u => u.DeletedAt == null)
                    .Select(u => new TopUser
                    {
                        Email = u.Email,
                        FullName = !string.IsNullOrWhiteSpace(u.FirstName) || !string.IsNullOrWhiteSpace(u.LastName)
                            ? $"{u.FirstName} {u.LastName}".Trim()
                            : u.Email,
                        CVCount = u.CVs.Count(cv => cv.DeletedAt == null),
                        TotalSpent = u.BillingHistories.Where(b => b.DeletedAt == null).Sum(b => b.Amount)
                    })
                    .OrderByDescending(u => u.CVCount)
                    .ThenByDescending(u => u.TotalSpent)
                    .Take(10)
                    .ToListAsync();

                // Revenue Analytics
                var monthStart = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, TimeSpan.Zero);
                var weekStart = now.AddDays(-7);
                var todayStart = new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, TimeSpan.Zero);

                var monthlyRevenue = await _context.BillingHistories
                    .Where(b => b.DeletedAt == null && b.CreatedAt >= monthStart)
                    .SumAsync(b => b.Amount);

                var weeklyRevenue = await _context.BillingHistories
                    .Where(b => b.DeletedAt == null && b.CreatedAt >= weekStart)
                    .SumAsync(b => b.Amount);

                var todayRevenue = await _context.BillingHistories
                    .Where(b => b.DeletedAt == null && b.CreatedAt >= todayStart)
                    .SumAsync(b => b.Amount);

                var totalTransactions = await _context.BillingHistories
                    .Where(b => b.DeletedAt == null)
                    .CountAsync();

                var monthlyTransactions = await _context.BillingHistories
                    .Where(b => b.DeletedAt == null && b.CreatedAt >= monthStart)
                    .CountAsync();

                var averageOrderValue = totalTransactions > 0 ? totalRevenue / totalTransactions : 0;

                // Revenue by Package
                var revenueByPackage = await _context.BillingHistories
                    .Where(b => b.DeletedAt == null)
                    .Include(b => b.Package)
                    .GroupBy(b => new { b.Package.Id, b.Package.Name })
                    .Select(g => new RevenueByPackage
                    {
                        PackageName = g.Key.Name,
                        Revenue = g.Sum(b => b.Amount),
                        Count = g.Count()
                    })
                    .OrderByDescending(r => r.Revenue)
                    .ToListAsync();

                var totalPackageRevenue = revenueByPackage.Sum(r => r.Revenue);
                foreach (var pkg in revenueByPackage)
                {
                    pkg.Percentage = totalPackageRevenue > 0 ? (pkg.Revenue / totalPackageRevenue) * 100 : 0;
                }

                // Top Paying Users
                var topPayingUsers = await _context.Users
                    .Where(u => u.DeletedAt == null)
                    .Select(u => new TopPayingUser
                    {
                        FullName = !string.IsNullOrWhiteSpace(u.FirstName) || !string.IsNullOrWhiteSpace(u.LastName)
                            ? $"{u.FirstName} {u.LastName}".Trim()
                            : u.Email,
                        Email = u.Email,
                        TotalSpent = u.BillingHistories.Where(b => b.DeletedAt == null).Sum(b => b.Amount),
                        TransactionCount = u.BillingHistories.Count(b => b.DeletedAt == null)
                    })
                    .Where(u => u.TotalSpent > 0)
                    .OrderByDescending(u => u.TotalSpent)
                    .Take(10)
                    .ToListAsync();

                // Revenue chart data (last 30 days)
                var last30Days = Enumerable.Range(0, 30).Select(i => now.AddDays(-29 + i).Date).ToList();
                var revenueChartData = new RevenueChartData
                {
                    Labels = last30Days.Select(d => d.ToString("MMM dd")).ToList(),
                    Values = new List<decimal>()
                };

                foreach (var day in last30Days)
                {
                    var nextDay = day.AddDays(1);
                    var dayUtc = day.ToUniversalTime();
                    var nextDayUtc = nextDay.ToUniversalTime();

                    var dayRevenue = await _context.BillingHistories
                        .Where(b => b.DeletedAt == null && b.CreatedAt >= dayUtc && b.CreatedAt < nextDayUtc)
                        .SumAsync(b => (decimal?)b.Amount) ?? 0;

                    revenueChartData.Values.Add(dayRevenue);
                }

                // Add mock data if no real revenue data exists
                if (revenueChartData.Values.All(v => v == 0))
                {
                    var random = new Random();
                    revenueChartData.Values = last30Days.Select(_ => (decimal)(random.Next(50, 500) + random.NextDouble())).ToList();
                }

                // Add mock data for revenue by package if empty
                if (!revenueByPackage.Any())
                {
                    var mockPackages = new[] { "Basic", "Pro", "Enterprise", "Premium" };
                    var random = new Random();
                    revenueByPackage = mockPackages.Select(name => new RevenueByPackage
                    {
                        PackageName = name,
                        Revenue = random.Next(500, 5000),
                        Count = random.Next(10, 100)
                    }).ToList();

                    var mockTotal = revenueByPackage.Sum(r => r.Revenue);
                    foreach (var pkg in revenueByPackage)
                    {
                        pkg.Percentage = mockTotal > 0 ? (pkg.Revenue / mockTotal) * 100 : 0;
                    }
                }

                // Add mock data for top paying users if empty
                if (!topPayingUsers.Any())
                {
                    var mockNames = new[] { "John Doe", "Jane Smith", "Mike Johnson", "Sarah Williams", "David Brown" };
                    var random = new Random();
                    topPayingUsers = mockNames.Select(name => new TopPayingUser
                    {
                        FullName = name,
                        Email = name.Replace(" ", ".").ToLower() + "@example.com",
                        TotalSpent = random.Next(100, 2000),
                        TransactionCount = random.Next(5, 20)
                    }).OrderByDescending(u => u.TotalSpent).ToList();
                }

                var viewModel = new DashboardViewModel
                {
                    Stats = new DashboardStats
                    {
                        TotalUsers = totalUsers,
                        ActiveUsers = activeUsers,
                        TotalCVs = totalCVs,
                        TotalJobs = totalJobs,
                        PendingJobs = pendingJobs,
                        RunningJobs = runningJobs,
                        SucceededJobs = succeededJobs,
                        FailedJobs = failedJobs,
                        TotalRevenue = totalRevenue,
                        SuccessRate = successRate,
                        UserGrowthPercent = userGrowth,
                        CVGrowthPercent = cvGrowth,
                        RevenueGrowthPercent = revenueGrowth
                    },
                    RecentActivities = recentActivities,
                    UserGrowthData = userGrowthData,
                    CVCreationData = cvCreationData,
                    RevenueData = revenueChartData,
                    TopUsers = topUsers,
                    RevenueAnalytics = new RevenueAnalytics
                    {
                        TotalRevenue = totalRevenue,
                        MonthlyRevenue = monthlyRevenue,
                        WeeklyRevenue = weeklyRevenue,
                        TodayRevenue = todayRevenue,
                        AverageOrderValue = averageOrderValue,
                        TotalTransactions = totalTransactions,
                        MonthlyTransactions = monthlyTransactions,
                        RevenueByPackage = revenueByPackage,
                        TopPayingUsers = topPayingUsers
                    }
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard");
                return View(new DashboardViewModel());
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
