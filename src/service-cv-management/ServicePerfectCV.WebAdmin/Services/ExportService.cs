using System.Text;
using ServicePerfectCV.WebAdmin.Models.Users;
using ServicePerfectCV.WebAdmin.Models.CVs;

namespace ServicePerfectCV.WebAdmin.Services
{
    public class ExportService
    {
        public byte[] ExportUsersToCsv(List<UserListViewModel> users)
        {
            var csv = new StringBuilder();

            // Header
            csv.AppendLine("ID,Full Name,Email,Role,Status,Total Credit,Used Credit,Remaining Credit,CV Count,Registered Date,Updated Date");

            // Data rows
            foreach (var user in users)
            {
                csv.AppendLine($"{EscapeCsv(user.Id.ToString())}," +
                             $"{EscapeCsv(user.FullName)}," +
                             $"{EscapeCsv(user.Email)}," +
                             $"{EscapeCsv(user.Role.ToString())}," +
                             $"{EscapeCsv(user.Status.ToString())}," +
                             $"{user.TotalCredit}," +
                             $"{user.UsedCredit}," +
                             $"{user.RemainingCredit}," +
                             $"{user.CVCount}," +
                             $"{EscapeCsv(user.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"))}," +
                             $"{EscapeCsv(user.UpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? "Never")}");
            }

            return Encoding.UTF8.GetBytes(csv.ToString());
        }

        public byte[] ExportCVsToCsv(List<CVListViewModel> cvs)
        {
            var csv = new StringBuilder();

            // Header
            csv.AppendLine("ID,Title,User Email,User Name,Is Structured,Has PDF,Education Count,Experience Count,Skill Count,Project Count,Certification Count,Created Date,Updated Date");

            // Data rows
            foreach (var cv in cvs)
            {
                csv.AppendLine($"{EscapeCsv(cv.Id.ToString())}," +
                             $"{EscapeCsv(cv.Title)}," +
                             $"{EscapeCsv(cv.UserEmail)}," +
                             $"{EscapeCsv(cv.UserFullName)}," +
                             $"{cv.IsStructuredDone}," +
                             $"{cv.HasPdf}," +
                             $"{cv.EducationCount}," +
                             $"{cv.ExperienceCount}," +
                             $"{cv.SkillCount}," +
                             $"{cv.ProjectCount}," +
                             $"{cv.CertificationCount}," +
                             $"{EscapeCsv(cv.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"))}," +
                             $"{EscapeCsv(cv.UpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A")}");
            }

            return Encoding.UTF8.GetBytes(csv.ToString());
        }

        private static string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            }

            return value;
        }
    }
}

