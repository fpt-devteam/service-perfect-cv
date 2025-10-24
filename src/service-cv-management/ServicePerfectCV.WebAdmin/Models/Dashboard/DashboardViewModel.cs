namespace ServicePerfectCV.WebAdmin.Models.Dashboard
{
  public class DashboardViewModel
  {
    public DashboardStats Stats { get; set; } = new();
    public List<RecentActivity> RecentActivities { get; set; } = new();
    public ChartData UserGrowthData { get; set; } = new();
    public ChartData CVCreationData { get; set; } = new();
    public RevenueChartData RevenueData { get; set; } = new();
    public List<TopUser> TopUsers { get; set; } = new();
    public RevenueAnalytics RevenueAnalytics { get; set; } = new();
  }

  public class DashboardStats
  {
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int TotalCVs { get; set; }
    public int TotalJobs { get; set; }
    public int PendingJobs { get; set; }
    public int RunningJobs { get; set; }
    public int SucceededJobs { get; set; }
    public int FailedJobs { get; set; }
    public decimal TotalRevenue { get; set; }
    public double SuccessRate { get; set; }

    // Growth percentages
    public double UserGrowthPercent { get; set; }
    public double CVGrowthPercent { get; set; }
    public double RevenueGrowthPercent { get; set; }
  }

  public class RecentActivity
  {
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public DateTimeOffset Timestamp { get; set; }
    public string Icon { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
  }

  public class ChartData
  {
    public List<string> Labels { get; set; } = new();
    public List<int> Values { get; set; } = new();
  }

  public class TopUser
  {
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public int CVCount { get; set; }
    public decimal TotalSpent { get; set; }
  }

  public class RevenueChartData
  {
    public List<string> Labels { get; set; } = new();
    public List<decimal> Values { get; set; } = new();
  }

  public class RevenueAnalytics
  {
    public decimal TotalRevenue { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public decimal WeeklyRevenue { get; set; }
    public decimal TodayRevenue { get; set; }
    public decimal AverageOrderValue { get; set; }
    public int TotalTransactions { get; set; }
    public int MonthlyTransactions { get; set; }
    public List<RevenueByPackage> RevenueByPackage { get; set; } = new();
    public List<TopPayingUser> TopPayingUsers { get; set; } = new();
  }

  public class RevenueByPackage
  {
    public string PackageName { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public int Count { get; set; }
    public decimal Percentage { get; set; }
  }

  public class TopPayingUser
  {
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal TotalSpent { get; set; }
    public int TransactionCount { get; set; }
  }
}

