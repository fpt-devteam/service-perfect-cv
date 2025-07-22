using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Interfaces
{
    public interface IPushNotificationService
    {
        Task SendAsync(IEnumerable<string> deviceTokens, string title, string message);
    }
}
